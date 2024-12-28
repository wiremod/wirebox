using System;
using Sandbox.UI;
using System.Text.Json;
using Sandbox.UI.Construct;

namespace Sandbox.Tools
{
	[Library( "tool_wiring", Title = "Wiring", Description = "Wire entities together", Group = "construction" )]
	public partial class WiringTool : BaseTool
	{
		private GameObject inputEnt { get; set; }
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

		private int InputPortIndex { get; set; } = 0;
		private int OutputPortIndex { get; set; } = 0;

		[ConVar( "tool_wiring_model" )]
		public static string _ { get; set; } = "models/wirebox/katlatze/chip_rectangle.vmdl";
		[ConVar( "tool_wiring_materialgroup" )]
		public static int _2 { get; set; } = 0;

		protected override bool IsPreviewTraceValid( SceneTraceResult tr )
		{
			return !IsProxy && Input.Down( "drop" );
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if ( IsProxy ) return;

			Description = CalculateDescription();

			var tr = Parent.BasicTraceTool();
			UpdateTraceEntPorts( tr );

			if ( Input.Pressed( "attack1" ) )
			{
				if ( !tr.Hit || !tr.Body.IsValid() || !tr.GameObject.IsValid() || tr.GameObject.IsWorld() )
					return;

				if ( !inputEnt.IsValid() )
				{
					// stage 1
					if ( tr.GameObject.GetComponent<IWireInputComponent>() is not IWireInputComponent wireProp || wireProp.GetInputNames().Length == 0 )
					{
						return;
					}
					inputEnt = tr.GameObject;
					inputPos = tr.EndPosition;
					Parent.ToolEffects( tr.EndPosition, tr.Normal );
				}
				else
				{
					// stage 2
					if ( inputEnt.GetComponent<IWireInputComponent>() is not IWireInputComponent wireInputProp )
						return;
					if ( tr.GameObject.GetComponent<IWireOutputComponent>() is not IWireOutputComponent wireOutputProp || wireOutputProp.GetOutputNames().Length == 0 )
						return;
					string outputName;
					string inputName;
					try
					{
						outputName = wireOutputProp.GetOutputNames()[OutputPortIndex];
						inputName = wireInputProp.GetInputNames()[InputPortIndex];
					}
					catch ( IndexOutOfRangeException )
					{
						Reset();
						return;
					}

					// Log.Info( "Wiring " + wireInputProp + "'s " + inputName + " to " + wireOutputProp + "'s " + outputName );
					wireOutputProp.WireConnect( wireInputProp, outputName, inputName );

					var attachEnt = tr.Body.IsValid() ? tr.Body.GetGameObject() : tr.GameObject;
					var ropeParticle = Particles.MakeParticleSystem( "particles/wirebox/wire.vpcf", inputEnt.WorldTransform, 0, inputEnt );
					var RopePoints = new List<ParticleControlPoint>();

					var p = new GameObject();
					p.SetParent( inputEnt );
					p.LocalPosition = inputEnt.Transform.World.PointToLocal( inputPos );
					RopePoints.Add( new() { StringCP = "0", Value = ParticleControlPoint.ControlPointValueInput.GameObject, GameObjectValue = p } );

					var p2 = new GameObject();
					p2.SetParent( tr.GameObject );
					p2.LocalPosition = tr.Body.Transform.PointToLocal( tr.EndPosition );
					RopePoints.Add( new() { StringCP = "1", Value = ParticleControlPoint.ControlPointValueInput.GameObject, GameObjectValue = p2 } );

					ropeParticle.ControlPoints = RopePoints;
					wireInputProp.WirePorts.inputs[inputName].AttachRope = new WireCable( ropeParticle, inputEnt, attachEnt );
					Reset();
				}
			}
			else if ( Input.Pressed( "attack2" ) )
			{
				var portDirection = Input.Down( "run" ) ? -1 : 1;

				if ( inputEnt.IsValid() && inputEnt.GetComponent<IWireInputComponent>() is not null )
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
				if ( Stage == 0 && tr.GameObject.IsValid() && tr.GameObject.GetComponent<IWireInputComponent>() is IWireInputComponent wireEntity )
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
					ConsoleSystem.Run( "tool_current", "tool_debugger" );
					return;
				}
			}
			else
			{
				return;
			}

			Parent.ToolEffects( tr.EndPosition, tr.Normal );
		}

		protected void UpdateTraceEntPorts( SceneTraceResult tr )
		{
			if ( inputEnt.IsValid() && inputEnt.GetComponent<IWireInputComponent>() is IWireInputComponent wireInputEnt )
			{
				wiringHud?.SetInputs( wireInputEnt.GetInputNames( true ), true, InputPortIndex );
				if ( tr.GameObject.IsValid() && tr.GameObject.GetComponent<IWireOutputComponent>() is IWireOutputComponent wireOutputProp1 )
				{
					OutputPortIndex = Math.Clamp( OutputPortIndex - Input.MouseWheel.y.FloorToInt(), 0, Math.Max( 0, wireOutputProp1.GetOutputNames().Length - 1 ) );
					wiringHud?.SetOutputs( wireOutputProp1.GetOutputNames( true ), true, OutputPortIndex );
				}
				else
				{
					wiringHud?.SetOutputs( [] );
				}
			}
			else
			{
				if ( tr.GameObject.IsValid() && tr.GameObject.GetComponent<IWireInputComponent>() is IWireInputComponent wireInputEnt2 )
				{
					InputPortIndex = Math.Clamp( InputPortIndex - Input.MouseWheel.y.FloorToInt(), 0, Math.Max( 0, wireInputEnt2.GetInputNames().Length - 1 ) );
					wiringHud?.SetInputs( wireInputEnt2.GetInputNames( true ), false, InputPortIndex );
				}
				else
				{
					wiringHud?.SetInputs( [] );
				}
				if ( tr.GameObject.IsValid() && tr.GameObject.GetComponent<IWireOutputComponent>() is IWireOutputComponent wireOutputProp2 )
				{
					wiringHud?.SetOutputs( wireOutputProp2.GetOutputNames( true ), false, OutputPortIndex );
				}
				else
				{
					wiringHud?.SetOutputs( [] );
				}
			}
		}

		public override void Reset()
		{
			inputEnt = null;
			InputPortIndex = 0;
			OutputPortIndex = 0;
			wiringHud?.SetInputs( [] );
			wiringHud?.SetOutputs( [] );
		}

		private string CalculateDescription()
		{
			var drop = Input.GetButtonOrigin( "drop", true ) == null ? Input.GetButtonOrigin( "drop" ) : "G";
			var desc = $"Connect wirable entities with wires.\nHold {drop} to spawn Gates.\n{Input.GetButtonOrigin( "run" )} - {Input.GetButtonOrigin( "flashlight" )} for Debugger.\n";
			if ( Stage == 0 )
			{
				desc += $"\n{Input.GetButtonOrigin( "attack1" )}: select Input";
				desc += $"\n{Input.GetButtonOrigin( "attack2" )}: scroll to next Input ({Input.GetButtonOrigin( "run" )} for previous)";
				desc += "\nScroll Wheel: scroll between Inputs";
				desc += $"\n{Input.GetButtonOrigin( "reload" )}: Disconnect Input";
			}
			else if ( Stage == 1 )
			{
				desc += $"\n{Input.GetButtonOrigin( "attack1" )}: select Output";
				desc += $"\n{Input.GetButtonOrigin( "attack2" )}: scroll to next Output ({Input.GetButtonOrigin( "run" )} for previous)";
				desc += "\nScroll Wheel: scroll between Outputs";
				desc += $"\n{Input.GetButtonOrigin( "reload" )}: Cancel";
			}
			return desc;
		}

		public override void Activate()
		{
			base.Activate();

			if ( !IsProxy )
			{
				SandboxHud.Instance.Panel.ChildrenOfType<WiringHud>().ToList().ForEach( x => x.Delete() );
				wiringHud = SandboxHud.Instance.Panel.AddChild<WiringHud>();
				wireGatePanel = SandboxHud.Instance.Panel.AddChild<WireGateHud>();

				var modelSelector = new ModelSelector( new string[] { "gate", "controller" } );
				SpawnMenu.Instance?.ToolPanel?.AddChild( modelSelector );
			}
			Reset();
		}

		public override void Disabled()
		{
			base.Disabled();
			if ( !IsProxy )
			{
				wireGatePanel?.Delete();
				wiringHud?.Delete();
			}
			Reset();
		}

		[ConCmd( "wire_spawn_gate" )]  // todo: should this be an RPC?
		public static void SpawnGate( string gateType )
		{
			var owner = Player.FindLocalPlayer();

			var tr = Player.DoBasicTrace();

			if ( tr.GameObject.GetComponent<WireGateComponent>() is WireGateComponent wireGateEntity )
			{
				wireGateEntity.Update( gateType );
				if ( owner.Inventory.ActiveWeapon is ToolGun toolgun && toolgun.CurrentTool is WiringTool wiringTool )
				{
					toolgun.ToolEffects( tr.EndPosition, tr.Normal );
				}
				return;
			}

			var go = new GameObject()
			{
				WorldPosition = tr.HitPosition,
				WorldRotation = Rotation.LookAt( tr.Normal, tr.Direction ) * Rotation.From( new Angles( 90, 0, 0 ) ),
			};
			var prop = go.AddComponent<Prop>();
			prop.Model = Model.Load( ConsoleSystem.GetValue( "tool_wiring_model" ) );

			var propHelper = go.AddComponent<PropHelper>();
			var gate = go.AddComponent<WireGateComponent>();
			gate.GateType = gateType;

			int.TryParse( ConsoleSystem.GetValue( "tool_wiring_materialgroup" ), out int matGroup );
			propHelper.MaterialGroupIndex = matGroup;

			UndoSystem.Add( creator: owner, callback: () =>
			{
				go.Destroy();
				return $"Undid Gate {gateType} creation";
			}, prop: go );

			go.NetworkSpawn();
			go.Network.SetOrphanedMode( NetworkOrphaned.Host );

			var attachEnt = tr.Body.IsValid() ? tr.Body.GetGameObject() : tr.GameObject;
			if ( attachEnt.IsValid() )
			{
				go.SetParent( attachEnt );
			}
			if ( owner.Inventory.ActiveWeapon is ToolGun toolgun2 && toolgun2.CurrentTool is WiringTool wiringTool2 )
			{
				toolgun2.ToolEffects( tr.EndPosition, tr.Normal );
			}
			// Event.Run( "entity.spawned", ent, owner );
		}

		public static void SpawnlistsInitialize()
		{
			ModelSelector.AddToSpawnlist( "gate", new string[] {
				Cloud.Asset("https://asset.party/facepunch/metal_fences_gate_small"), // lol get it
			} );
		}
	}
}
