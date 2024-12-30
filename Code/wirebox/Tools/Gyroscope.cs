namespace Sandbox.Tools
{
	[Library( "tool_wiregyroscope", Title = "Gyroscope", Description = "Create a Wire Gyroscope for retrieving rotation data", Group = "construction" )]
	public partial class GyroscopeTool : BaseSpawnTool
	{
		[Property, Title( "Model" ), ModelProperty( SpawnLists = ["gyroscope", "controller"] )]
		public override string SpawnModel { get; set; } = "models/wirebox/katlatze/gyroscope.vmdl";

		protected override TypeDescription GetSpawnedComponent()
		{
			return TypeLibrary.GetType<WireGyroscopeComponent>();
		}
	}
}
