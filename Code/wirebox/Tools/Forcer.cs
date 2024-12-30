namespace Sandbox.Tools
{
	[Library( "tool_wireforcer", Title = "Wire Forcer", Description = "Create a Wire Forcer for pushing/pulling props", Group = "construction" )]
	public partial class ForcerTool : BaseSpawnTool
	{
		[Property, Title( "Model" ), ModelProperty( SpawnLists = ["ranger", "forcer", "controller"] )]
		public override string SpawnModel { get; set; } = "models/wirebox/katlatze/apc.vmdl";
		[Property, Title( "Laser Length" ), Range( 10f, 1000f, 1f )]
		public float Length { get; set; } = 100f;

		protected override TypeDescription GetSpawnedComponent()
		{
			return TypeLibrary.GetType<WireForcerComponent>();
		}
		protected override void UpdateEntity( GameObject go )
		{
			var forcer = go.GetComponent<WireForcerComponent>();
			forcer.Length = Length;
		}
	}
}
