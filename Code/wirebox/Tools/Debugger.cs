namespace Sandbox.Tools
{
	[Library( "tool_debugger", Title = "Wiring Debugger", Description = "Shows selected wire ports on the HUD", Group = "constraints" )]
	public partial class DebuggerTool : WiringTool
	{
		public static HashSet<IWireComponent> TrackedEntities { get; set; } = new();

		protected DebuggerHud debuggerHud;

		protected override void OnUpdate()
		{
			if ( !IsProxy )
			{
				var tr = Parent.BasicTraceTool();

				UpdateTraceEntPorts( tr );

				if ( Input.Pressed( "attack1" ) )
				{
					if ( !tr.Hit || !tr.Body.IsValid() || !tr.GameObject.IsValid() || tr.GameObject.IsWorld() )
						return;
					if ( tr.GameObject.GetComponent<IWireComponent>() is not IWireComponent wireProp )
						return;

					TrackedEntities.Add( wireProp );
				}
				else if ( Input.Pressed( "attack2" ) )
				{
					if ( !tr.Hit || !tr.Body.IsValid() || !tr.GameObject.IsValid() || tr.GameObject.IsWorld() )
						return;
					if ( tr.GameObject.GetComponent<IWireComponent>() is not IWireComponent wireProp )
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
				Parent.ToolEffects( tr.EndPosition, tr.Normal );
			}
		}

		public override void Activate()
		{
			if ( !IsProxy )
			{
				Description = $"Shows selected wire ports on the HUD.\n{Input.GetButtonOrigin( "run" )} - {Input.GetButtonOrigin( "flashlight" )} for Wiring tool.\n";
				Description += $"\n{Input.GetButtonOrigin( "attack1" )}: Add entity to HUD";
				Description += $"\n{Input.GetButtonOrigin( "attack2" )}: Remove entity from HUD";
				Description += $"\n{Input.GetButtonOrigin( "reload" )}: Clear HUD";

				SandboxHud.Instance.Panel.ChildrenOfType<DebuggerHud>().ToList().ForEach( x => x.Delete() );
				debuggerHud = SandboxHud.Instance.Panel.AddChild<DebuggerHud>();
			}

			base.Activate();
		}

		public override void Disabled()
		{
			base.Disabled();
			if ( !IsProxy )
			{
				if ( TrackedEntities.Count == 0 )
				{
					debuggerHud?.Delete();
				}
			}
		}
	}
}
