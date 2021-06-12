using System;
namespace Sandbox.Tools
{
	[Library( "tool_button", Title = "Button", Description = "Create Buttons! Shift for Toggle buttons", Group = "construction" )]
	public partial class ButtonTool : BaseWireTool
	{
		protected override Type GetEntityType()
		{
			return typeof( ButtonEntity );
		}
		protected override string GetModel()
		{
			return "models/wirebox/katlatze/button.vmdl";
		}
		protected override ModelEntity SpawnEntity( TraceResult tr )
		{
			return new ButtonEntity {
				Position = tr.EndPos,
				Rotation = Rotation.LookAt( tr.Normal, tr.Direction ) * Rotation.From( new Angles( 90, 0, 0 ) ),
				IsToggle = Input.Down(InputButton.Run),
			};
		}
	}
}
