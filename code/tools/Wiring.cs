using System;
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

		private WireGateHud wireGatePanel;
		private WiringHud wiringHud;

		private int Stage
		{
			get
			{
				if ( !inputEnt.IsValid() ) return 0;
				else { return 1; }
			}
		}

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
			if ( Game.IsClient )
			{
				Description = CalculateDescription();
			}
			using ( Prediction.Off() )
			{
				var tr = DoTrace();

				UpdateTraceEntPorts( tr );

				if ( Input.Pressed( "attack1" ) )
				{
					if ( !tr.Hit || !tr.Body.IsValid() || !tr.Entity.IsValid() || tr.Entity.IsWorld )
						return;

					if ( !inputEnt.IsValid() )
					{
						// stage 1
						if ( tr.Entity is not IWireInputEntity wireProp || wireProp.GetInputNames().Length == 0 )
						{
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
							string outputName;
							string inputName;
							try
							{
								outputName = wireOutputProp.GetOutputNames()[OutputPortIndex];
								inputName = wireInputProp.GetInputNames()[InputPortIndex];
							}
							catch ( IndexOutOfRangeException e )
							{
								Reset();
								return;
							}

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
					if ( Stage == 0 && tr.Entity is IWireInputEntity wireEntity && Game.IsServer )
					{
						wireEntity.DisconnectInput( wireEntity.GetInputNames()[InputPortIndex] );
					}
					else
					{
						Reset();
					}
				}
				else if ( Input.Pressed( "flashlight" ) )
				{
					if ( Input.Down( "run" ) )
					{
						if ( Game.IsClient )
						{
							ConsoleSystem.Run( "tool_current", "tool_debugger" );
							return;
						}
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

		protected void UpdateTraceEntPorts( TraceResult tr )
		{
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
		}

		private void Reset()
		{
			inputEnt = null;
			InputPortIndex = 0;
			OutputPortIndex = 0;
			ShowInputs( null );
			ShowOutputs( null );
		}

		private string CalculateDescription()
		{
			var desc = $"Connect wirable entities with wires.\nHold G to spawn Gates.\nShift-F for Debugger.\n";
			if ( Stage == 0 )
			{
				desc += "\nPrimary: select Input";
				desc += "\nSecondary: scroll to next Input (shift for previous)";
				desc += "\nScroll Wheel: scroll between Inputs";
				desc += "\nReload: Disconnect Input";
			}
			else if ( Stage == 1 )
			{
				desc += "\nPrimary: select Output";
				desc += "\nSecondary: scroll to next Output (shift for previous)";
				desc += "\nScroll Wheel: scroll between Outputs";
				desc += "\nReload: Cancel";
			}
			return desc;
		}

		public override void Activate()
		{
			base.Activate();

			if ( Game.IsClient )
			{
				wiringHud = SandboxHud.Instance.RootPanel.AddChild<WiringHud>();
				wireGatePanel = SandboxHud.Instance.RootPanel.AddChild<WireGateHud>();

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

		[Event( "spawnlists.initialize" )]
		public static void SpawnlistsInitialize()
		{
			ModelSelector.AddToSpawnlist( "gate", new string[] {
				Cloud.Asset("https://asset.party/facepunch/metal_fences_gate_small"), // lol get it
			} );
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
}
