using Sandbox.Tools;
using Wirebox.Components;
namespace Wirebox.Tools;

[Library( "tool_wirekeyboard", Title = "Keyboard", Description = "Create Wire Keyboards for reading Player input", Group = "construction" )]
public partial class KeyboardTool : BaseSpawnTool
{
	[Property, Title( "Model" ), ModelProperty( SpawnLists = ["keyboard", "controller", "button"] )]
	public override string SpawnModel { get; set; } = "models/wirebox/katlatze/button.vmdl";
	protected override TypeDescription GetSpawnedComponent()
	{
		return TypeLibrary.GetType<WireKeyboardComponent>();
	}
}
