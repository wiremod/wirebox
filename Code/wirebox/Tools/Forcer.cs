namespace Sandbox.Tools
{
	[Library( "tool_wireforcer", Title = "Wire Forcer", Description = "Create a Wire Forcer for pushing/pulling props", Group = "construction" )]
	public partial class ForcerTool : BaseSpawnTool
	{
		[ConVar( "tool_wireforcer_model" )]
		public static string _ { get; set; } = "models/wirebox/katlatze/apc.vmdl";
		[ConVar( "tool_wireforcer_length" )]
		public static float _2 { get; set; } = 100f;

		protected override TypeDescription GetSpawnedComponent()
		{
			return TypeLibrary.GetType<WireForcerComponent>();
		}
		protected override void UpdateEntity( GameObject go )
		{
			var forcer = go.GetComponent<WireForcerComponent>();
			forcer.Length = float.Parse( GetConvarValue( "tool_wireforcer_length" ) );
		}
		protected override string[] GetSpawnLists()
		{
			return new string[] { "ranger", "controller" };
		}

		public override void CreateToolPanel()
		{
			var toolConfigUi = new ForcerToolConfig();
			SpawnMenu.Instance?.ToolPanel?.AddChild( toolConfigUi );
		}
	}
}
