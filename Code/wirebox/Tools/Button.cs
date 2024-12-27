using System;
namespace Sandbox.Tools
{
	[Library( "tool_wirebutton", Title = "Wire Button", Description = "Create Buttons! Shift for Toggle buttons", Group = "construction" )]
	public partial class ButtonTool : BaseSpawnTool
	{
		[ConVar( "tool_wirebutton_model" )]
		public static string _ { get; set; } = "models/wirebox/katlatze/button.vmdl";

		protected override TypeDescription GetSpawnedComponent()
		{
			return TypeLibrary.GetType<WireButtonComponent>();
		}
		protected override void UpdateEntity( GameObject go )
		{
			base.UpdateEntity( go );

			var button = go.GetComponent<WireButtonComponent>();
			button.IsToggle = Input.Down( "run" );
		}
		protected override string[] GetSpawnLists()
		{
			return ["button"];
		}
	}
}
