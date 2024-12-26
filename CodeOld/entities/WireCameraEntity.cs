using Sandbox;

[Library( "ent_wirecamera", Title = "Wire Camera" )]
public partial class WireCameraEntity : Prop, IWireOutputEntity
{
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();
	private SceneCamera sceneCamera;

	public PortType[] WireGetOutputs()
	{
		return new PortType[] {
			PortType.Entity("Self"),
		};
	}

	public void WireInitializeOutputs()
	{
		this.WireTriggerOutput( "Self", this as Entity );
	}

	public SceneCamera GetSceneCamera()
	{
		Game.AssertClient();
		if ( sceneCamera == null )
		{
			sceneCamera = new SceneCamera
			{
				World = Game.SceneWorld,
				FieldOfView = 90, // todo: make customizable
				ZFar = 10000,
				ZNear = 1,
				FirstPersonViewer = this,
			};
		}
		sceneCamera.Position = Position + Rotation.Up * 2 + Rotation.Forward * 2;
		sceneCamera.Rotation = Rotation;
		return sceneCamera;
	}
}
