namespace Sandbox.Tools
{
	[Library( "tool_wiregyroscope", Title = "Wire Gyroscope", Description = "Create a Wire Gyroscope for retrieving rotation data", Group = "construction" )]
	public partial class GyroscopeTool : BaseSpawnTool
	{
		[Property, Title( "Model" ), ModelProperty( SpawnLists = ["gyroscope", "controller"] )]
		public override string SpawnModel { get; set; } = "models/citizen_props/icecreamcone01.vmdl";

		protected override TypeDescription GetSpawnedComponent()
		{
			return TypeLibrary.GetType<WireGyroscopeComponent>();
		}
	}
}
