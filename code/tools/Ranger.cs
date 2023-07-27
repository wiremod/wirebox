using System;
namespace Sandbox.Tools
{
	[Library( "tool_wireranger", Title = "Wire Ranger", Description = "Create a Wire Ranger for running traces", Group = "construction" )]
	public partial class RangerTool : BaseWireTool
	{
		[ConVar.ClientData( "tool_wireranger_model" )]
		public static string _ { get; set; } = "models/wirebox/katlatze/apc.vmdl";
		protected override Type GetEntityType()
		{
			return typeof( WireRangerEntity );
		}
		protected override ModelEntity SpawnEntity( TraceResult tr )
		{
			return new WireRangerEntity {
				Position = tr.EndPos,
				Rotation = Rotation.LookAt( tr.Normal, tr.Direction ) * Rotation.From( new Angles( 90, 0, 0 ) )
			};
		}
		protected override string[] GetSpawnLists()
		{
			return new string[] { "ranger", "controller" };
		}
	}
}
