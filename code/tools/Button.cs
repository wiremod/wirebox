using System;
namespace Sandbox.Tools
{
	[Library( "tool_wirebutton", Title = "Wire Button", Description = "Create Buttons! Shift for Toggle buttons", Group = "construction" )]
	public partial class ButtonTool : BaseWireTool
	{
		[ConVar.ClientData( "tool_wirebutton_model" )]
		public static string _ { get; set; } = "models/wirebox/katlatze/button.vmdl";

		protected override Type GetEntityType()
		{
			return typeof( WireButtonEntity );
		}
		protected override ModelEntity SpawnEntity( TraceResult tr )
		{
			var ent = (WireButtonEntity)base.SpawnEntity( tr );
			ent.IsToggle = Input.Down( "run" );
			return ent;
		}
		protected override string[] GetSpawnLists()
		{
			return new string[] { "button" };
		}
	}
}
