[Library( "ent_wiregyroscope", Title = "Wire Gyroscope" )]
public partial class WireGyroscopeComponent : BaseWireOutputComponent
{
	public override PortType[] WireGetOutputs()
	{
		return new PortType[] {
			PortType.Float("Pitch"),
			PortType.Float("Yaw"),
			PortType.Float("Roll"),
			PortType.Angle("Angle"),
			PortType.Rotation("Rotation"),
		};
	}

	protected override void OnFixedUpdate()
	{
		if ( !this.IsValid() )
			return;

		var outputs = WirePorts.outputs;
		if ( !outputs.ContainsKey( "Angle" ) )
		{
			return;
		}
		var angle = WorldRotation.Angles();
		if ( outputs["Angle"].value is Angles oldValue && oldValue.Equals( angle ) )
		{
			return;
		}
		this.WireTriggerOutput( "Pitch", angle.pitch );
		this.WireTriggerOutput( "Yaw", angle.yaw );
		this.WireTriggerOutput( "Roll", angle.roll );
		this.WireTriggerOutput( "Angle", angle );
		this.WireTriggerOutput( "Rotation", WorldRotation );
	}
}
