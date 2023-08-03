using System;
namespace Sandbox.Tools
{
	[Library( "tool_wirespeedometer", Title = "Wire Speedometer", Description = "Create a Wire Speedometer for retrieving velocity data", Group = "construction" )]
	public partial class SpeedometerTool : BaseWireTool
	{
		[ConVar.ClientData( "tool_wirespeedometer_model" )]
		public static string _ { get; set; } = "models/wirebox/katlatze/apc.vmdl";

		protected override Type GetEntityType()
		{
			return typeof( WireSpeedometerEntity );
		}
		protected override ModelEntity SpawnEntity( TraceResult tr )
		{
			return new WireSpeedometerEntity
			{
				Position = tr.EndPosition,
				Rotation = Rotation.LookAt( tr.Normal, tr.Direction ) * Rotation.From( new Angles( 90, 0, 0 ) )
			};
		}
		protected override string[] GetSpawnLists()
		{
			return new string[] { "speedometer", "controller" };
		}
	}
}
