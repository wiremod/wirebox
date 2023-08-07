using System;
using System.Linq;
using Sandbox.UI;
using System.Text.Json;
using Sandbox.UI.Construct;

namespace Sandbox.Tools
{
	[Library( "tool_wiring", Title = "Wiring", Description = "Wire entities together", Group = "construction" )]
	public partial class WiringTool : BaseTool
	{
		[Net, Local]
		private Entity inputEnt { get; set; }
		[Net, Local]
		private Vector3 inputPos { get; set; }

		private WireGatePanel wireGatePanel;
		private WiringPanel wiringHud;

		// Cache the inputs/outputs here, so we can network them to the client, as only the server knows the current port values
		// These would be tidier over in the HUD class, but [Net] seems buggy over there
		[Net, Local]
		public string NetInputs { get; set; } = "";
		[Net, Local]
		public string NetOutputs { get; set; } = "";

		[Net, Local]
		private int InputPortIndex { get; set; } = 0;
		[Net, Local]
		private int OutputPortIndex { get; set; } = 0;

		[ConVar.ClientData( "tool_wiring_model" )]
		public static string _ { get; set; } = "models/wirebox/katlatze/chip_rectangle.vmdl";
		[ConVar.ClientData( "tool_wiring_materialgroup" )]
		public static int _2 { get; set; } = 0;

		public override void Simulate()
		{
			using ( Prediction.Off() )
			{
				var startPos = Owner.EyePosition;
				var dir = Owner.EyeRotation.Forward;

				var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
					.Ignore( Owner )
					.Run();


				if ( inputEnt is IWireInputEntity wireInputEnt )
				{
					ShowInputs( wireInputEnt, true );
					if ( tr.Entity is IWireOutputEntity wireOutputProp1 )
					{
						if ( Game.IsServer )
						{
							OutputPortIndex = Math.Clamp( OutputPortIndex - Input.MouseWheel, 0, Math.Max( 0, wireOutputProp1.GetOutputNames().Length - 1 ) );
						}
						ShowOutputs( wireOutputProp1, true );
					}
					else
					{
						ShowOutputs( null );
					}
				}
				else
				{
					if ( tr.Entity is IWireInputEntity wireInputEnt2 )
					{
						if ( Game.IsServer )
						{
							InputPortIndex = Math.Clamp( InputPortIndex - Input.MouseWheel, 0, Math.Max( 0, wireInputEnt2.GetInputNames().Length - 1 ) );
						}
						ShowInputs( wireInputEnt2, false );
					}
					else
					{
						ShowInputs( null, false );
					}
					if ( tr.Entity is IWireOutputEntity wireOutputProp2 )
					{
						ShowOutputs( wireOutputProp2 );
					}
					else
					{
						ShowOutputs( null );
					}
				}



				if ( Input.Pressed( "attack1" ) )
				{
					Log.Info( "Clicked" );

					if ( !tr.Hit || !tr.Body.IsValid() || !tr.Entity.IsValid() || tr.Entity.IsWorld )
						return;

					if ( !inputEnt.IsValid() )
					{
						// stage 1

						Log.Info( "in stage 1" );
						if ( tr.Entity is not IWireInputEntity wireProp || wireProp.GetInputNames().Length == 0 )
						{
							Log.Info( "in early return" );
							return;
						}
						if ( Game.IsClient )
						{
							CreateHitEffects( tr.EndPosition, tr.Normal );
							return;
						}
						inputEnt = tr.Entity;
						inputPos = tr.EndPosition;
					}
					else
					{
						// stage 2
						if ( inputEnt is not IWireInputEntity wireInputProp )
							return;
						if ( tr.Entity is not IWireOutputEntity wireOutputProp || wireOutputProp.GetOutputNames().Length == 0 )
							return;

						if ( Game.IsServer )
						{
							var outputName = wireOutputProp.GetOutputNames()[OutputPortIndex];
							var inputName = wireInputProp.GetInputNames()[InputPortIndex];

							// Log.Info("Wiring " + wireInputProp + "'s " + inputName + " to " + wireOutputProp + "'s " + outputName);
							wireOutputProp.WireConnect( wireInputProp, outputName, inputName );

							var attachEnt = tr.Body.IsValid() ? tr.Body.GetEntity() : tr.Entity;
							var rope = new WireCable( "particles/wirebox/wire.vpcf", inputEnt, attachEnt );
							rope.Particle.SetEntity( 0, inputEnt, inputEnt.Transform.PointToLocal( inputPos ) );

							var attachLocalPos = tr.Body.Transform.PointToLocal( tr.EndPosition );
							if ( attachEnt.IsWorld )
							{
								rope.Particle.SetPosition( 1, attachLocalPos );
							}
							else
							{
								rope.Particle.SetEntityBone( 1, attachEnt, tr.Bone, new Transform( attachLocalPos ) );
							}
							wireInputProp.WirePorts.inputs[inputName].AttachRope = rope;
						}
						Reset();
					}
				}
				else if ( Input.Pressed( "attack2" ) )
				{
					if ( Game.IsClient )
					{
						return;
					}
					var portDirection = Input.Down( "run" ) ? -1 : 1;

					if ( inputEnt is IWireInputEntity )
					{
						OutputPortIndex += portDirection;
					}
					else
					{
						InputPortIndex += portDirection;
					}

					return;
				}
				else if ( Input.Pressed( "reload" ) )
				{
					if ( tr.Entity is IWireInputEntity wireEntity && Game.IsServer )
					{
						wireEntity.DisconnectInput( wireEntity.GetInputNames()[InputPortIndex] );
					}
					else
					{
						Reset();
					}
				}
				else
				{
					return;
				}
				if ( Game.IsClient )
				{
					CreateHitEffects( tr.EndPosition, tr.Normal );
				}
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

			if ( Game.IsClient )
			{
				SandboxHud.Instance.RootPanel.StyleSheet.Load( "/ui/wiringhud.scss" );
				wiringHud = SandboxHud.Instance.RootPanel.AddChild<WiringPanel>();
				wireGatePanel = SandboxHud.Instance.RootPanel.AddChild<WireGatePanel>( "wire-gate-menu" );

				var modelSelector = new ModelSelector( new string[] { "gate", "controller" } );
				SpawnMenu.Instance?.ToolPanel?.AddChild( modelSelector );
			}
			Reset();
		}

		public override void Deactivate()
		{
			base.Deactivate();
			if ( Game.IsClient )
			{
				wireGatePanel?.Delete( true );
				wiringHud?.Delete();
			}
			Reset();
		}

		[ConCmd.Server( "wire_spawn_gate" )]
		public static void SpawnGate( string gateType )
		{
			var owner = ConsoleSystem.Caller?.Pawn as Player;

			if ( ConsoleSystem.Caller == null )
				return;

			var tr = Trace.Ray( owner.EyePosition, owner.EyePosition + owner.EyeRotation.Forward * 500 )
			  .UseHitboxes()
			  .Ignore( owner )
			  .Size( 2 )
			  .Run();

			if ( tr.Entity is WireGateEntity wireGateEntity )
			{
				wireGateEntity.Update( gateType );
				if ( owner.Inventory.Active is Tool toolgun && toolgun.CurrentTool is WiringTool wiringTool )
				{
					wiringTool.CreateHitEffects( tr.EndPosition, tr.Normal );
				}
				return;
			}

			var ent = new WireGateEntity
			{
				Position = tr.EndPosition,
				Rotation = Rotation.LookAt( tr.Normal, tr.Direction ) * Rotation.From( new Angles( 90, 0, 0 ) ),
				GateType = gateType,
			};
			ent.SetModel( ConsoleSystem.Caller.GetClientData<string>( "tool_wiring_model" ) );
			int.TryParse( ConsoleSystem.Caller.GetClientData<string>( "tool_wiring_materialgroup" ), out int matGroup );
			ent.SetMaterialGroup( matGroup );

			var attachEnt = tr.Body.IsValid() ? tr.Body.GetEntity() : tr.Entity;
			if ( attachEnt.IsValid() )
			{
				ent.SetParent( attachEnt, tr.Body.GroupName );
			}
			if ( owner.Inventory.Active is Tool toolgun2 && toolgun2.CurrentTool is WiringTool wiringTool2 )
			{
				wiringTool2.CreateHitEffects( tr.EndPosition, tr.Normal );
			}
			Event.Run( "entity.spawned", ent, owner );
		}


		// A wrapper around wiringHud.SetInputs that helps sync the server port state to the client for display
		private void ShowInputs( IWireInputEntity ent, bool entSelected = false )
		{
			string[] names = Array.Empty<string>();
			if ( ent != null )
			{
				if ( Game.IsServer )
				{
					names = ent.GetInputNames( true );
					NetInputs = JsonSerializer.Serialize( names ); // serialize em, as [Net] errors on string[]'s
				}
				else
				{
					names = NetInputs?.Length > 0 ? JsonSerializer.Deserialize<string[]>( NetInputs ) : ent.GetInputNames();
				}
			}
			if ( Game.IsClient )
			{
				wiringHud?.SetInputs( names, entSelected, InputPortIndex );
			}
		}
		private void ShowOutputs( IWireOutputEntity ent, bool selectingOutput = false )
		{
			string[] names = Array.Empty<string>();
			if ( ent != null )
			{
				if ( Game.IsServer )
				{
					names = ent.GetOutputNames( true );
					NetOutputs = JsonSerializer.Serialize( names );
				}
				else
				{
					names = NetOutputs?.Length > 0 ? JsonSerializer.Deserialize<string[]>( NetOutputs ) : ent.GetOutputNames();
				}
			}

			if ( Game.IsClient )
			{
				wiringHud?.SetOutputs( names, selectingOutput, OutputPortIndex );
			}
		}
	}

	public partial class WiringPanel : Panel
	{
		private string[] lastInputs = System.Array.Empty<string>();
		private string[] lastOutputs = System.Array.Empty<string>();
		public Panel InputsPanel { get; set; }
		public Panel OutputsPanel { get; set; }

		public WiringPanel()
		{
			SetTemplate( "/ui/wiringhud.html" );
			InputsPanel = GetChild( 0 ).GetChild( 0 );
			OutputsPanel = GetChild( 0 ).GetChild( 1 );
		}

		public void SetInputs( string[] names, bool selected = false, int portIndex = 0 )
		{
			if ( Game.LocalClient.Pawn is SandboxPlayer sandboxPlayer )
			{
				sandboxPlayer.SuppressScrollWheelInventory = names.Length != 0;
			}
			if ( !InputsPanel.IsValid() )
			{
				return;
			}
			foreach ( var lineItem in InputsPanel.GetChild( 1 ).Children )
			{
				lineItem.SetClass( "active", InputsPanel.GetChild( 1 ).GetChildIndex( lineItem ) == portIndex );
			}
			InputsPanel.SetClass( "selected", selected );
			if ( Enumerable.SequenceEqual( lastInputs, names ) )
			{
				return;
			}
			lastInputs = names;
			InputsPanel.GetChild( 1 ).DeleteChildren( true );

			foreach ( var name in names )
			{
				InputsPanel.GetChild( 1 ).AddChild<Label>( "port" ).SetText( name );
			}
		}
		public void SetOutputs( string[] names, bool selectingOutput = false, int portIndex = 0 )
		{
			if ( !OutputsPanel.IsValid() )
			{
				return;
			}
			foreach ( var lineItem in OutputsPanel.GetChild( 1 ).Children )
			{
				lineItem.SetClass( "active", selectingOutput && OutputsPanel.GetChild( 1 ).GetChildIndex( lineItem ) == portIndex );
			}
			OutputsPanel.SetClass( "selected", selectingOutput );
			if ( Enumerable.SequenceEqual( lastOutputs, names ) )
			{
				return;
			}
			lastOutputs = names;
			OutputsPanel.GetChild( 1 ).DeleteChildren( true );
			OutputsPanel.SetClass( "visible", names.Length != 0 );

			foreach ( var name in names )
			{
				OutputsPanel.GetChild( 1 ).AddChild<Label>( "port" ).SetText( name );
			}
		}
	}

	public partial class WireGatePanel : Panel
	{
		public WireGatePanel()
		{
			var container = Add.Panel( "wire-gate-container" );
			foreach ( var kvp in WireGateEntity.GetGates() )
			{
				var category = kvp.Key;
				var gates = kvp.Value;

				var categoryRow = container.Add.Panel( "wire-gate-category" );
				var categoryText = categoryRow.Add.TextEntry( category );
				categoryText.AddClass( "wire-gate-category-label" );
				var categoryList = categoryRow.Add.Panel( "wire-gate-category-list" );
				foreach ( var gateName in gates )
				{
					categoryList.Add.Button( gateName, () =>
					{
						ConsoleSystem.Run( "wire_spawn_gate", gateName );
					} );
				}
			}
		}
		public override void Tick()
		{
			base.Tick();
			SetClass( "visible", Input.Down( "drop" ) );
		}
	}
}
