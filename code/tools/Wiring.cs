using System;
using System.Linq;
using Sandbox.UI;

namespace Sandbox.Tools
{
	[Library( "tool_wiring", Title = "Wiring", Description = "Wire entities together", Group = "construction" )]
	public partial class WiringTool : BaseTool
	{
		private Entity inputEnt;
		private WiringHud wiringHud;

		private int InputPortIndex = 0;
		private int OutputPortIndex = 0;
		public override void Simulate()
		{
			using ( Prediction.Off() ) {
				var startPos = Owner.EyePos;
				var dir = Owner.EyeRot.Forward;


				var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
					.Ignore( Owner )
					.Run();

				if ( inputEnt is WireInputEntity wireInputProp1 ) {
					wiringHud.SetInputs( wireInputProp1.GetInputNames(), true );
					if ( tr.Entity is WireOutputEntity wireOutputProp1 ) {
						OutputPortIndex = Math.Clamp( OutputPortIndex - Input.MouseWheel, 0, wireOutputProp1.GetOutputNames().Length - 1 );
						wiringHud.SetOutputs( wireOutputProp1.GetOutputNames(), OutputPortIndex );
					}
					else {
						wiringHud.SetOutputs( Array.Empty<string>() );
					}
				}
				else if ( tr.Entity is WireInputEntity wireInputProp2 ) {
					InputPortIndex = Math.Clamp( InputPortIndex - Input.MouseWheel, 0, wireInputProp2.GetInputNames().Length - 1 );
					wiringHud.SetInputs( wireInputProp2.GetInputNames(), false, InputPortIndex );
				}
				else {
					wiringHud.SetInputs( Array.Empty<string>(), false );
				}


				if ( Input.Pressed( InputButton.Attack1 ) ) {

					if ( !tr.Hit || !tr.Body.IsValid() || !tr.Entity.IsValid() || tr.Entity.IsWorld )
						return;

					if ( !inputEnt.IsValid() ) {
						// stage 1

						if ( tr.Entity is not WireInputEntity wireProp )
							return;
						inputEnt = tr.Entity;
					}
					else {
						// stage 2
						if ( inputEnt is not WireInputEntity wireInputProp )
							return;
						if ( tr.Entity is not WireOutputEntity wireOutputProp )
							return;

						if ( Host.IsServer ) {
							var outputName = wireOutputProp.GetOutputNames()[OutputPortIndex];
							var inputName = wireInputProp.GetInputNames()[InputPortIndex];

							// Log.Info("Wiring " + wireInputProp + "'s " + inputName + " to " + wireOutputProp + "'s " + outputName);
							wireOutputProp.WireConnect( wireInputProp, outputName, inputName );
							wireOutputProp.WireTriggerOutput( outputName, wireOutputProp.GetOutput( outputName ).value );
						}
						Reset();
					}
				}
				else if ( Input.Pressed( InputButton.Attack2 ) ) {
					var portDirection = Input.Down( InputButton.Run ) ? -1 : 1;

					if ( inputEnt is WireInputEntity ) {
						OutputPortIndex += portDirection;
					}
					else {
						InputPortIndex += portDirection;
					}

					return;
				}
				else if ( Input.Pressed( InputButton.Reload ) ) {
					if ( tr.Entity is WireInputEntity wireEntity ) {
						wireEntity.DisconnectInput( wireEntity.GetInputNames()[InputPortIndex] );
					}
					else {
						Reset();
					}
				}
				else {
					return;
				}

				CreateHitEffects( tr.EndPos );
			}
		}

		private void Reset()
		{
			inputEnt = null;
			InputPortIndex = 0;
			OutputPortIndex = 0;
			wiringHud?.SetInputs( Array.Empty<string>(), false );
			wiringHud?.SetOutputs( Array.Empty<string>() );
		}

		public override void Activate()
		{
			base.Activate();

			wiringHud = new WiringHud();
			Reset();
		}

		public override void Deactivate()
		{
			base.Deactivate();

			wiringHud.Delete();
			Reset();
		}
	}

	public partial class WiringHud : HudEntity<RootPanel>
	{
		private string[] lastInputs = System.Array.Empty<string>();
		private string[] lastOutputs = System.Array.Empty<string>();
		public Panel InputsPanel { get; set; }
		public Panel OutputsPanel { get; set; }

		public WiringHud()
		{
			if ( !IsClient ) {
				return;
			}

			RootPanel.SetTemplate( "/code/addons/wirebox/code/tools/WiringHud.html" );
			InputsPanel = RootPanel.GetChild( 0 ).GetChild( 0 );
			OutputsPanel = RootPanel.GetChild( 0 ).GetChild( 1 );
		}

		public void SetInputs( string[] names, bool selected = false, int portIndex = 0 )
		{
			if ( Local.Pawn is SandboxPlayer sandboxPlayer ) {
				sandboxPlayer.SuppressScrollWheelInventory = names.Length != 0;
			}
			if ( !IsClient || RootPanel == null ) {
				return;
			}
			foreach ( var lineItem in InputsPanel.GetChild( 1 ).Children ) {
				lineItem.SetClass( "active", InputsPanel.GetChild( 1 ).GetChildIndex( lineItem ) == portIndex );
			}
			InputsPanel.SetClass( "selected", selected );
			if ( Enumerable.SequenceEqual( lastInputs, names ) ) {
				return;
			}
			lastInputs = names;
			InputsPanel.GetChild( 1 ).DeleteChildren( true );

			foreach ( var name in names ) {
				InputsPanel.GetChild( 1 ).AddChild<Label>( "port" ).SetText( name );
			}
		}
		public void SetOutputs( string[] names, int portIndex = 0 )
		{
			if ( !IsClient || RootPanel == null ) {
				return;
			}
			foreach ( var lineItem in OutputsPanel.GetChild( 1 ).Children ) {
				lineItem.SetClass( "active", OutputsPanel.GetChild( 1 ).GetChildIndex( lineItem ) == portIndex );
			}
			if ( Enumerable.SequenceEqual( lastOutputs, names ) ) {
				return;
			}
			lastOutputs = names;
			OutputsPanel.GetChild( 1 ).DeleteChildren( true );
			OutputsPanel.SetClass( "visible", names.Length != 0 );

			foreach ( var name in names ) {
				OutputsPanel.GetChild( 1 ).AddChild<Label>( "port" ).SetText( name );
			}
		}
	}
}
