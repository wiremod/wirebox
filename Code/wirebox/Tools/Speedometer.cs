namespace Sandbox.Tools
{
	[Library( "tool_wirespeedometer", Title = "Wire Speedometer", Description = "Create a Wire Speedometer for retrieving velocity data", Group = "construction" )]
	public partial class SpeedometerTool : BaseSpawnTool
	{
		[ConVar( "tool_wirespeedometer_model" )]
		public static string _ { get; set; } = "models/wirebox/katlatze/apc.vmdl";
		protected override TypeDescription GetSpawnedComponent()
		{
			return TypeLibrary.GetType<WireSpeedometerComponent>();
		}
		protected override string[] GetSpawnLists()
		{
			return new string[] { "speedometer", "controller" };
		}
	}
}
