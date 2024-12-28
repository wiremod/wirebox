[Library( "ent_wiregps", Title = "Wire GPS" )]
public partial class WireGPSComponent : BaseWireOutputComponent
{	public override PortType[] WireGetOutputs()
	{
		return new PortType[] {
			PortType.Float("X"),
			PortType.Float("Y"),
			PortType.Float("Z"),
			PortType.Vector3("Position")
		};
	}

	protected override void OnFixedUpdate()
	{
		if ( !this.IsValid() )
			return;

		var outputs = WirePorts.outputs;
		if ( !outputs.ContainsKey( "Position" ) )
		{
			return;
		}
		if ( outputs["Position"].value is Vector3 oldValue && oldValue.Equals( WorldPosition ) )
		{
			return;
		}
		this.WireTriggerOutput( "X", WorldPosition.x );
		this.WireTriggerOutput( "Y", WorldPosition.y );
		this.WireTriggerOutput( "Z", WorldPosition.z );
		this.WireTriggerOutput( "Position", WorldPosition );
	}
}
