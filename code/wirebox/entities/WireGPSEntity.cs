using Sandbox;

[Library( "ent_wiregps", Title = "Wire GPS" )]
public partial class WireGPSEntity : Prop, WireOutputEntity
{
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();
	public PortType[] WireGetOutputs()
	{
		return new PortType[] {
			PortType.Float("X"),
			PortType.Float("Y"),
			PortType.Float("Z"),
			PortType.Vector3("Position")
		};
	}

	[Event.Physics.PostStep]
	public void OnPostPhysicsStep()
	{
		if ( !this.IsValid() )
			return;

		var outputs = ((IWireEntity)this).WirePorts.outputs;
		if ( !outputs.ContainsKey( "Position" ) ) {
			return;
		}
		if ( outputs["Position"].value is Vector3 oldValue && oldValue.Equals( Position ) ) {
			return;
		}
		this.WireTriggerOutput( "X", Position.x );
		this.WireTriggerOutput( "Y", Position.y );
		this.WireTriggerOutput( "Z", Position.z );
		this.WireTriggerOutput( "Position", Position );
	}
}
