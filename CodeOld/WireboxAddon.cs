using Sandbox;

[Library( "wirebox" )]
public static class WireboxAddon
{
	[Event( "game.init" )]
	[Event( "package.mounted" )]
	public static void Initialize()
	{
		Log.Info( "Init Wirebox" );
		Sandbox.Tools.ConstraintTool.CreateWireboxConstraintController = ConstraintControllerEntity.CreateFromTool;
	}
}
