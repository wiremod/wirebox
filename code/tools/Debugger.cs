using System.Linq;
using System.Collections.Generic;

namespace Sandbox.Tools
{
	[Library( "tool_debugger", Title = "Wire Debugger", Description = "Shows selected wire ports on the HUD", Group = "construction" )]
	public partial class DebuggerTool : WiringTool
	{
		public static HashSet<IWireEntity> TrackedEntities { get; set; } = new();

		protected DebuggerHud debuggerHud;

		public override void Simulate()
		{
			if ( !Game.IsClient )
			{
				return;
			}
			using ( Prediction.Off() )
			{
				var tr = DoTrace( false );

				UpdateTraceEntPorts( tr );

				if ( Input.Pressed( "attack1" ) )
				{
					if ( !tr.Hit || !tr.Body.IsValid() || !tr.Entity.IsValid() || tr.Entity.IsWorld )
						return;
					if ( tr.Entity is not IWireEntity wireProp )
						return;

					TrackedEntities.Add( wireProp );
				}
				else if ( Input.Pressed( "attack2" ) )
				{
					if ( !tr.Hit || !tr.Body.IsValid() || !tr.Entity.IsValid() || tr.Entity.IsWorld )
						return;
					if ( tr.Entity is not IWireEntity wireProp )
						return;

					TrackedEntities.Remove( wireProp );
				}
				else if ( Input.Pressed( "reload" ) )
				{
					TrackedEntities.Clear();
				}
				else if ( Input.Pressed( "flashlight" ) )
				{
					if ( Input.Down( "run" ) )
					{
						ConsoleSystem.Run( "tool_current", "tool_wiring" );
						return;
					}
				}
				else
				{
					return;
				}
				CreateHitEffects( tr.EndPosition, tr.Normal );
			}
		}

		public override void Activate()
		{
			if ( Game.IsClient )
			{
				Description = "Shows selected wire ports on the HUD.\nShift-F for Wiring tool.\n";
				Description += "\nPrimary: Add entity to HUD";
				Description += "\nSecondary: Remove entity from HUD";
				Description += "\nReload: Clear HUD";

				SandboxHud.Instance.RootPanel.ChildrenOfType<DebuggerHud>().ToList().ForEach( x => x.Delete() );
				debuggerHud = SandboxHud.Instance.RootPanel.AddChild<DebuggerHud>();
			}

			base.Activate();
		}

		public override void Deactivate()
		{
			base.Deactivate();
			if ( Game.IsClient )
			{
				if ( TrackedEntities.Count == 0 )
				{
					debuggerHud?.Delete();
				}
			}
		}
	}
}
