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
		var localVel = Transform.World.NormalToLocal( rigid.Velocity );
		if ( this.GetOutput( "Velocity" ).Equals( localVel ) )
		{
			return;
		}
		this.WireTriggerOutput( "Speed", localVel.Length );

		this.WireTriggerOutput( "X", localVel.x );
		this.WireTriggerOutput( "Y", localVel.y );
		this.WireTriggerOutput( "Z", localVel.z );
		this.WireTriggerOutput( "Velocity", localVel );

		this.WireTriggerOutput( "Pitch", rigid.AngularVelocity.x );
		this.WireTriggerOutput( "Yaw", rigid.AngularVelocity.y );
		this.WireTriggerOutput( "Roll", rigid.AngularVelocity.z );
		this.WireTriggerOutput( "AngularVelocity", rigid.AngularVelocity );
	}
}
