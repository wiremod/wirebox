using System;

namespace Sandbox.Tools
{
	[Library( "tool_wirekeyboard", Title = "Wire Keyboard", Description = "Create Wire Keyboards for reading Player input", Group = "construction" )]
	public partial class KeyboardTool : BaseWireTool
	{
		[ConVar.ClientData( "tool_wirekeyboard_model" )]
		public static string _ { get; set; } = "models/wirebox/katlatze/button.vmdl";
		protected override Type GetEntityType()
		{
			return typeof( KeyboardEntity );
		}
		protected override string[] GetSpawnLists()
		{
			return new string[] { "keyboard", "controller" };
		}
	}
}
