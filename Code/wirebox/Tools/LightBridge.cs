using Sandbox.Tools;
using Wirebox.Components;
namespace Wirebox.Tools;

[Library( "tool_wirelightbridge", Title = "Light Bridge", Description = "Create Hardlight Bridges with wireable lengths", Group = "construction" )]
public partial class LightBridgeTool : BaseSpawnTool
{
	// [Property, Title( "Model" ), ModelProperty( SpawnLists = ["lightbridge"] )]
	public override string SpawnModel { get; set; } = "models/wirebox/katlatze/lightbridge.vmdl";
	protected override TypeDescription GetSpawnedComponent()
	{
		return TypeLibrary.GetType<WireLightBridgeComponent>();
	}
}
