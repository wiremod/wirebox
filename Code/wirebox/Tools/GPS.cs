namespace Sandbox.Tools
{
	[Library( "tool_wiregps", Title = "Wire GPS", Description = "Create a Wire GPS for retrieving position data", Group = "construction" )]
	public partial class GPSTool : BaseSpawnTool
	{
		[Property, Title( "Model" ), ModelProperty( SpawnLists = ["gps", "controller"] )]
		public override string SpawnModel { get; set; } = "models/wirebox/katlatze/apc.vmdl";

		protected override TypeDescription GetSpawnedComponent()
		{
			return TypeLibrary.GetType<WireGPSComponent>();
		}
	}
}
