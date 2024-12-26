using System;
namespace Sandbox.Tools
{
	[Library( "tool_wireranger", Title = "Wire Ranger", Description = "Create a Wire Ranger for running traces", Group = "construction" )]
	public partial class RangerTool : BaseWireTool
	{
		[ConVar.ClientData( "tool_wireranger_model" )]
		public static string _ { get; set; } = "models/wirebox/katlatze/apc.vmdl";
		[ConVar.ClientData( "tool_wireranger_length" )]
		public static float _2 { get; set; } = 100f;
		[ConVar.ClientData( "tool_wireranger_defaultzero" )]
		public static bool _3 { get; set; } = true;

		protected override Type GetEntityType()
		{
			return typeof( WireRangerEntity );
		}
		protected override ModelEntity SpawnEntity( TraceResult tr )
		{
			var ent = (WireRangerEntity)base.SpawnEntity( tr );
			ent.Length = float.Parse( GetConvarValue( "tool_wireranger_length" ) );
			ent.DefaultZero = GetConvarValue( "tool_wireranger_defaultzero" ) != "0";
			return ent;
		}
		protected override string[] GetSpawnLists()
		{
			return new string[] { "ranger", "controller" };
		}

		public override void CreateToolPanel()
		{
			if ( Game.IsClient )
			{
				var toolConfigUi = new RangerToolConfig();
				SpawnMenu.Instance?.ToolPanel?.AddChild( toolConfigUi );
			}
		}
	}
}
