using System;
namespace Sandbox.Tools
{
	[Library( "tool_wiredigitalscreen", Title = "Wire Digital Screen", Description = "Create a Wire Digital Screen for displaying numbers", Group = "construction" )]
	public partial class DigitalScreenTool : BaseWireTool
	{
		[ConVar.ClientData( "tool_wiredigitalscreen_model" )]
		public static string _ { get; set; } = "models/television/flatscreen_tv.vmdl";

		protected override Type GetEntityType()
		{
			return typeof( WireDigitalScreenEntity );
		}
		protected override ModelEntity SpawnEntity( TraceResult tr )
		{
			return new WireDigitalScreenEntity
			{
				Position = tr.EndPosition,
				Rotation = Rotation.LookAt( tr.Normal, tr.Direction ) * Rotation.From( new Angles( 90, 0, 0 ) )
			};
		}
		protected override string[] GetSpawnLists()
		{
			return new string[] { "screen", "controller" };
		}
	}
}
