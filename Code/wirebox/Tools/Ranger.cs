namespace Sandbox.Tools
{
	[Library( "tool_wireranger", Title = "Wire Ranger", Description = "Create a Wire Ranger for running traces", Group = "construction" )]
	public partial class RangerTool : BaseSpawnTool
	{
		[ConVar( "tool_wireranger_model" )]
		public static string _ { get; set; } = "models/wirebox/katlatze/apc.vmdl";
		[ConVar( "tool_wireranger_length" )]
		public static float _2 { get; set; } = 100f;
		[ConVar( "tool_wireranger_defaultzero" )]
		public static bool _3 { get; set; } = true;

		protected override TypeDescription GetSpawnedComponent()
		{
			return TypeLibrary.GetType<WireRangerComponent>();
		}
		protected override void UpdateEntity( GameObject go )
		{
			var ranger = go.GetComponent<WireRangerComponent>();
			ranger.Length = float.Parse( GetConvarValue( "tool_wireranger_length" ) );
			ranger.DefaultZero = GetConvarValue( "tool_wireranger_defaultzero" ) != "0";
		}
		protected override string[] GetSpawnLists()
		{
			return new string[] { "ranger", "controller" };
		}

		public override void CreateToolPanel()
		{
			var toolConfigUi = new RangerToolConfig();
			SpawnMenu.Instance?.ToolPanel?.AddChild( toolConfigUi );
		}
	}
}
