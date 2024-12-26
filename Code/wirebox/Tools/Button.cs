using System;
namespace Sandbox.Tools
{
	[Library( "tool_wirebutton", Title = "Wire Button", Description = "Create Buttons! Shift for Toggle buttons", Group = "construction" )]
	public partial class ButtonTool : BaseSpawnTool
	{
		[ConVar( "tool_wirebutton_model" )]
		public static string _ { get; set; } = "models/wirebox/katlatze/button.vmdl";

		protected override bool IsMatchingEntity( GameObject go )
		{
			return go.GetComponent<WireButtonComponent>() != null;
		}
		protected override GameObject SpawnEntity( SceneTraceResult tr )
		{
			var go = base.SpawnEntity( tr );
			var button = go.AddComponent<WireButtonComponent>();
			button.IsToggle = Input.Down( "run" );

			UndoSystem.Add( creator: this.Owner, callback: () =>
			{
				go.Destroy();
				return "Undid button creation";
			}, prop: go );
			return go;
		}
		protected override string[] GetSpawnLists()
		{
			return ["button"];
		}
	}
}
