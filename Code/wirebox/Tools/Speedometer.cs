namespace Sandbox.Tools
{
	[Library( "tool_wirespeedometer", Title = "Speedometer", Description = "Create a Wire Speedometer for retrieving velocity data", Group = "construction" )]
	public partial class SpeedometerTool : BaseSpawnTool
	{
		[Property, Title( "Model" ), ModelProperty( SpawnLists = ["speedometer", "controller"] )]
		public override string SpawnModel { get; set; } = "models/wirebox/katlatze/apc.vmdl";
		protected override TypeDescription GetSpawnedComponent()
		{
			return TypeLibrary.GetType<WireSpeedometerComponent>();
		}
	}
}
