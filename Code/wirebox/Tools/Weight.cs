namespace Sandbox.Tools
{
	[Library( "tool_wireweight", Title = "Weight (Wire)", Description = "Create a Wire adjustable Weight. Can be negative, like a balloon!", Group = "construction" )]
	public partial class WireWeightTool : BaseSpawnTool
	{
		[Property, Title( "Model" ), ModelProperty( SpawnLists = ["weight", "controller"] )]
		public override string SpawnModel { get; set; } = "models/wirebox/katlatze/apc.vmdl";

		protected override TypeDescription GetSpawnedComponent()
		{
			return TypeLibrary.GetType<WireWeightComponent>();
		}
	}
}
