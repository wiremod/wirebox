namespace Sandbox.Tools
{
	[Library( "tool_wiredigitalscreen", Title = "Wire Digital Screen", Description = "Create a Wire Digital Screen for displaying numbers", Group = "construction" )]
	public partial class DigitalScreenTool : BaseSpawnTool
	{
		[ConVar( "tool_wiredigitalscreen_model" )]
		public static string _ { get; set; } = "models/television/flatscreen_tv.vmdl";

		protected override TypeDescription GetSpawnedComponent()
		{
			return TypeLibrary.GetType<WireDigitalScreenComponent>();
		}
		protected override string[] GetSpawnLists()
		{
			return new string[] { "screen", "controller" };
		}
	}
}
