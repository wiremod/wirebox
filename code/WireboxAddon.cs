using Sandbox;

[Library( "wirebox" )]
public class WireboxAddon : IAutoload
{
	public void Initialize()
	{
		Log.Info( "Init Wirebox" );
		WireCable.InitCleanupTimer();
		Sandbox.Tools.ConstraintTool.CreateWireboxConstraintController = ConstraintControllerEntity.CreateFromTool;
	}

	public void Dispose()
	{
		WireCable.StopCleanupTimer();
	}

}
