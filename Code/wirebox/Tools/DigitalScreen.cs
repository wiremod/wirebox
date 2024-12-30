namespace Sandbox.Tools
{
	[Library( "tool_wiredigitalscreen", Title = "Wire Digital Screen", Description = "Create a Wire Digital Screen for displaying numbers", Group = "construction" )]
	public partial class DigitalScreenTool : BaseSpawnTool
	{
		[Property, Title( "Screen Model" ), ModelProperty( SpawnLists = ["screen"] )]
		public override string SpawnModel { get; set; } = "models/television/flatscreen_tv.vmdl";

		protected override TypeDescription GetSpawnedComponent()
		{
			return TypeLibrary.GetType<WireDigitalScreenComponent>();
		}
	}
}
