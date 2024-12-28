[Library( "ent_wirespeedometer", Title = "Wire Speedometer" )]
public partial class WireSpeedometerComponent : BaseWireOutputComponent
{
	public override PortType[] WireGetOutputs()
	{
		return new PortType[] {
			PortType.Float("Speed"),
			PortType.Float("X"),
			PortType.Float("Y"),
			PortType.Float("Z"),
			PortType.Vector3("Velocity"),
			PortType.Float("Pitch"),
			PortType.Float("Yaw"),
			PortType.Float("Roll"),
			PortType.Vector3("AngularVelocity")
		};
	}

	protected override void OnFixedUpdate()
	{
		if ( !this.IsValid() )
			return;

		var rigid = GetComponent<Rigidbody>();
		if ( this.GetOutput( "Velocity" ).Equals( rigid.Velocity ) )
		{
			return;
		}
		this.WireTriggerOutput( "Speed", rigid.Velocity.Length );

		this.WireTriggerOutput( "X", rigid.Velocity.x );
		this.WireTriggerOutput( "Y", rigid.Velocity.y );
		this.WireTriggerOutput( "Z", rigid.Velocity.z );
		this.WireTriggerOutput( "Velocity", rigid.Velocity );

		this.WireTriggerOutput( "Pitch", rigid.AngularVelocity.x );
		this.WireTriggerOutput( "Yaw", rigid.AngularVelocity.y );
		this.WireTriggerOutput( "Roll", rigid.AngularVelocity.z );
		this.WireTriggerOutput( "AngularVelocity", rigid.AngularVelocity );
	}
}
