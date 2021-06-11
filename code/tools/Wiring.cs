using System;
using System.Linq;
using Sandbox.UI;
using System.Text.Json;

namespace Sandbox.Tools
{
	[Library( "tool_wiring", Title = "Wiring", Description = "Wire entities together", Group = "construction" )]
	public partial class WiringTool : BaseTool
	{
		private Entity inputEnt;
		private Vector3 inputPos;
		private WiringHud wiringHud;

		// Cache the inputs/outputs here, so we can network them to the client, as only the server knows the current port values
		// These would be tidier over in the HUD class, but [Net] seems buggy over there
		[Net]
		public string NetInputs { get; set; } = "";
		[Net]
		public string NetOutputs { get; set; } = "";
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


				if ( inputEnt is WireInputEntity wireInputEnt ) {
					ShowInputs( wireInputEnt, true );
					if ( tr.Entity is WireOutputEntity wireOutputProp1 ) {
						OutputPortIndex = Math.Clamp( OutputPortIndex - Input.MouseWheel, 0, wireOutputProp1.GetOutputNames().Length - 1 );
						ShowOutputs( wireOutputProp1 );
					}
					else {
						ShowOutputs( null );
					}
				}
				else if ( tr.Entity is WireInputEntity wireInputEnt2 ) {
					InputPortIndex = Math.Clamp( InputPortIndex - Input.MouseWheel, 0, wireInputEnt2.GetInputNames().Length - 1 );
					ShowInputs( wireInputEnt2, false );
				}
				else {
					ShowInputs( null, false );
				}



				if ( Input.Pressed( InputButton.Attack1 ) ) {

					if ( !tr.Hit || !tr.Body.IsValid() || !tr.Entity.IsValid() || tr.Entity.IsWorld )
						return;

					if ( !inputEnt.IsValid() ) {
						// stage 1

						if ( tr.Entity is not WireInputEntity wireProp )
							return;
						inputEnt = tr.Entity;
						inputPos = tr.EndPos;
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

							var rope = Particles.Create( "particles/wirebox/wire.vpcf" );
							rope.SetEntity( 0, inputEnt, inputEnt.Transform.PointToLocal( inputPos ) );

							var attachEnt = tr.Body.IsValid() ? tr.Body.Entity : tr.Entity;
							var attachLocalPos = tr.Body.Transform.PointToLocal( tr.EndPos );
							if ( attachEnt.IsWorld ) {
								rope.SetPos( 1, attachLocalPos );
							}
							else {
								rope.SetEntityBone( 1, attachEnt, tr.Bone, new Transform( attachLocalPos ) );
							}
							wireInputProp.WirePorts.AttachRope = rope;
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
				else if ( Input.Pressed( InputButton.Drop ) ) {
					if ( tr.Entity.GetType().IsSubclassOf( typeof( WireGateEntity ) ) ) {
						return;
					}
					if ( Host.IsServer ) {
						var ent = new WireGateEntity {
							Position = tr.EndPos,
							GateType = "Add",
						};
						ent.SetModel( "models/citizen_props/hotdog01.vmdl" );

						var attachEnt = tr.Body.IsValid() ? tr.Body.Entity : tr.Entity;
						if ( attachEnt.IsValid() ) {
							ent.SetParent( tr.Body.Entity, tr.Body.PhysicsGroup.GetBodyBoneName( tr.Body ) );
						}
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
			ShowInputs( null );
			ShowOutputs( null );
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

		// A wrapper around wiringHud.SetInputs that helps sync the server port state to the client for display
		private void ShowInputs( WireInputEntity ent, bool entSelected = false )
		{
			string[] names = Array.Empty<string>();
			if ( ent != null ) {
				if ( Host.IsServer ) {
					names = ent.GetInputNames( true );
					NetInputs = JsonSerializer.Serialize( names ); // serialize em, as [Net] errors on string[]'s
				}
				else {
					names = NetInputs != null ? JsonSerializer.Deserialize<string[]>( NetInputs ) : ent.GetInputNames();
				}
			}

			wiringHud?.SetInputs( names, entSelected, InputPortIndex );
		}
		private void ShowOutputs( WireOutputEntity ent )
		{
			string[] names = Array.Empty<string>();
			if ( ent != null ) {
				if ( Host.IsServer ) {
					names = ent.GetOutputNames( true );
					NetOutputs = JsonSerializer.Serialize( names );
				}
				else {
					names = NetOutputs != null ? JsonSerializer.Deserialize<string[]>( NetOutputs ) : ent.GetOutputNames();
				}
			}

			wiringHud?.SetOutputs( names, OutputPortIndex );
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
