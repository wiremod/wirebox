using System;
namespace Sandbox.Tools
{
	[Library( "tool_wirebutton", Title = "Button", Description = "Create Buttons! Shift for Toggle buttons", Group = "construction" )]
	public partial class ButtonTool : BaseWireTool
	{
		[ConVar.ClientData( "tool_button_model" )]
		public static string _ { get; set; } = "models/wirebox/katlatze/button.vmdl";

		protected override Type GetEntityType()
		{
			return typeof( WireButtonEntity );
		}
		protected override ModelEntity SpawnEntity( TraceResult tr )
		{
			return new WireButtonEntity
			{
				Position = tr.EndPosition,
				Rotation = Rotation.LookAt( tr.Normal, tr.Direction ) * Rotation.From( new Angles( 90, 0, 0 ) ),
				IsToggle = Input.Down( "run" ),
			};
		}
	}
}
