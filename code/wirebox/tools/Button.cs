using System;
namespace Sandbox.Tools
{
	[Library( "tool_button", Title = "Button", Description = "Create Buttons! Shift for Toggle buttons", Group = "construction" )]
	public partial class ButtonTool : BaseWireTool
	{
		[ConVar.ClientData( "tool_button_model" )]
		public static string _ { get; set; } = "models/wirebox/katlatze/button.vmdl";

		protected override Type GetEntityType()
		{
			return typeof( ButtonEntity );
		}
		protected override ModelEntity SpawnEntity( TraceResult tr )
		{
			return new ButtonEntity {
				Position = tr.EndPos,
				Rotation = Rotation.LookAt( tr.Normal, tr.Direction ) * Rotation.From( new Angles( 90, 0, 0 ) ),
				IsToggle = Input.Down( InputButton.Run ),
			};
		}
	}
	[Library]
	public class ButtonModels : MinimalExtended.IAutoload
	{
		public ButtonModels()
		{
			UI.ModelSelector.AddToSpawnlist( "button", new string[] {
				"models/wirebox/katlatze/button.vmdl",
				"models/citizen_props/coin01.vmdl",
			} );
		}
		public bool ReloadOnHotload => false;
		public void Dispose() { }
	}
}
