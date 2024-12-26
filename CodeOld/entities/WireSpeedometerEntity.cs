using Sandbox;

[Library( "ent_wirespeedometer", Title = "Wire Speedometer" )]
public partial class WireSpeedometerEntity : Prop, IWireOutputEntity
{
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();
	public PortType[] WireGetOutputs()
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
 
	[GameEvent.Physics.PostStep]
	public void OnPostPhysicsStep()
	{
		if ( !this.IsValid() )
			return;

		var localVel = Transform.NormalToLocal( Velocity );
		if ( ((IWireOutputEntity)this).GetOutput( "Velocity" ).Equals( localVel ) )
		{
			return;
		}
		this.WireTriggerOutput( "Speed", localVel.Length );

		this.WireTriggerOutput( "X", localVel.x );
		this.WireTriggerOutput( "Y", localVel.y );
		this.WireTriggerOutput( "Z", localVel.z );
		this.WireTriggerOutput( "Velocity", localVel );

		this.WireTriggerOutput( "Pitch", AngularVelocity.pitch );
		this.WireTriggerOutput( "Yaw", AngularVelocity.yaw );
		this.WireTriggerOutput( "Roll", AngularVelocity.roll );
		this.WireTriggerOutput( "AngularVelocity", AngularVelocity );
	}
}
