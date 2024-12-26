﻿using System;
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

		// private WireGateHud wireGatePanel;
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

		protected override void OnUpdate()
		{
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
					if ( tr.GameObject.GetComponent<BaseWireInputComponent>() is not BaseWireInputComponent wireProp || wireProp.GetInputNames().Length == 0 )
					{
						return;
					}
					inputEnt = tr.GameObject;
					inputPos = tr.EndPosition;
					Parent.ToolEffects( tr.EndPosition );
				}
				else
				{
					// stage 2
					if ( inputEnt.GetComponent<BaseWireInputComponent>() is not BaseWireInputComponent wireInputProp )
						return;
					if ( tr.GameObject.GetComponent<BaseWireOutputComponent>() is not BaseWireOutputComponent wireOutputProp || wireOutputProp.GetOutputNames().Length == 0 )
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

				if ( inputEnt.IsValid() && inputEnt.GetComponent<BaseWireInputComponent>() is not null )
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
				if ( Stage == 0 && tr.GameObject.IsValid() && tr.GameObject.GetComponent<BaseWireInputComponent>() is BaseWireInputComponent wireEntity )
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

			Parent.ToolEffects( tr.EndPosition );
		}

		protected void UpdateTraceEntPorts( SceneTraceResult tr )
		{
			if ( inputEnt.IsValid() && inputEnt.GetComponent<BaseWireInputComponent>() is BaseWireInputComponent wireInputEnt )
			{
				wiringHud?.SetInputs( wireInputEnt.GetInputNames( true ), true, InputPortIndex );
				if ( tr.GameObject.IsValid() && tr.GameObject.GetComponent<BaseWireOutputComponent>() is BaseWireOutputComponent wireOutputProp1 )
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
				if ( tr.GameObject.IsValid() && tr.GameObject.GetComponent<BaseWireInputComponent>() is BaseWireInputComponent wireInputEnt2 )
				{
					InputPortIndex = Math.Clamp( InputPortIndex - Input.MouseWheel.y.FloorToInt(), 0, Math.Max( 0, wireInputEnt2.GetInputNames().Length - 1 ) );
					wiringHud?.SetInputs( wireInputEnt2.GetInputNames( true ), false, InputPortIndex );
				}
				else
				{
					wiringHud?.SetInputs( [] );
				}
				if ( tr.GameObject.IsValid() && tr.GameObject.GetComponent<BaseWireOutputComponent>() is BaseWireOutputComponent wireOutputProp2 )
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
				// 	wireGatePanel = SandboxHud.Instance.Panel.AddChild<WireGateHud>();

				// 	var modelSelector = new ModelSelector( new string[] { "gate", "controller" } );
				// 	SpawnMenu.Instance?.ToolPanel?.AddChild( modelSelector );
			}
			Reset();
		}

		public override void Disabled()
		{
			base.Disabled();
			if ( !IsProxy )
			{
				// 	wireGatePanel?.Delete();
				wiringHud?.Delete();
			}
			Reset();
		}

		[ConCmd( "wire_spawn_gate" )]  // todo: should this be an RPC?
		public static void SpawnGate( string gateType )
		{
			/*
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
			*/
		}

		/*
		[Event( "spawnlists.initialize" )]
		public static void SpawnlistsInitialize()
		{
			ModelSelector.AddToSpawnlist( "gate", new string[] {
				Cloud.Asset("https://asset.party/facepunch/metal_fences_gate_small"), // lol get it
			} );
		}
		*/
	}
}
