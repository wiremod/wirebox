using System;
namespace Sandbox.Tools
{
	[Library( "tool_wirekeyboard", Title = "Wire Keyboard", Description = "Create Wire Keyboards for reading Player input", Group = "construction" )]
	public partial class KeyboardTool : BaseWireTool
	{
		protected override Type GetEntityType()
		{
			return typeof( KeyboardEntity );
		}
		protected override string GetModel()
		{
			return "models/wirebox/katlatze/button.vmdl";
		}
		protected override ModelEntity SpawnEntity( TraceResult tr )
		{
			var ent = new KeyboardEntity {
				Position = tr.EndPos,
				Rotation = Rotation.LookAt( tr.Normal, tr.Direction ) * Rotation.From( new Angles( 90, 0, 0 ) ),
			};

			ent.UpdateEntity( (SandboxPlayer)Owner );

			return ent;
		}

		protected override void UpdateEntity( Entity ent )
		{
			((KeyboardEntity)ent).UpdateEntity( Owner );
		}
	}
}
