namespace Sandbox.Tools
{
	[Library( "tool_wireranger", Title = "Wire Ranger", Description = "Create a Wire Ranger for running traces", Group = "construction" )]
	public partial class RangerTool : BaseSpawnTool
	{
		[Property, Title( "Model" ), ModelProperty( SpawnLists = ["ranger", "controller"] )]
		public override string SpawnModel { get; set; } = "models/wirebox/katlatze/apc.vmdl";
		[Property, Title( "Laser Length" ), Range( 10f, 1000f, 1f )]
		public float Length { get; set; } = 100f;
		[Property, Title( "Default to Zero" )]
		public bool DefaultZero { get; set; } = true;

		protected override TypeDescription GetSpawnedComponent()
		{
			return TypeLibrary.GetType<WireRangerComponent>();
		}
		protected override void UpdateEntity( GameObject go )
		{
			var ranger = go.GetComponent<WireRangerComponent>();
			ranger.Length = Length;
			ranger.DefaultZero = DefaultZero;
		}
	}
}
