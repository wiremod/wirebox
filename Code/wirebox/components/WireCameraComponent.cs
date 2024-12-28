[Library( "ent_wirecamera", Title = "Wire Camera" )]
public partial class WireCameraComponent : BaseWireOutputComponent
{
	private SceneCamera sceneCamera;

	public override PortType[] WireGetOutputs()
	{
		return new PortType[] {
			PortType.GameObject("Self"),
		};
	}

	public override void WireInitializeOutputs()
	{
		this.WireTriggerOutput( "Self", GameObject );
	}

	public SceneCamera GetSceneCamera()
	{
		if ( sceneCamera == null )
		{
			sceneCamera = new SceneCamera
			{
				World = Scene.SceneWorld,
				FieldOfView = 90, // todo: make customizable
				ZFar = 10000,
				ZNear = 1,
			};
		}
		sceneCamera.Position = WorldPosition + WorldRotation.Up * 2 + WorldRotation.Forward * 2;
		sceneCamera.Rotation = WorldRotation;
		return sceneCamera;
	}
}
