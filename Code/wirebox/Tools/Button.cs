namespace Sandbox.Tools
{
	[Library( "tool_wirebutton", Title = "Button", Description = "Create Buttons! Shift for Toggle buttons", Group = "construction" )]
	public partial class ButtonTool : BaseSpawnTool
	{
		[Property, Title( "Model" ), ModelProperty( SpawnLists = ["button"] )]
		public override string SpawnModel { get; set; } = "models/wirebox/katlatze/button.vmdl";

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
	}
}
