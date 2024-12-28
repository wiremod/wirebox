using Sandbox;

[Library( "ent_wireweightscale", Title = "Wire Weight Scale" )]
public partial class WireWeightScaleComponent : BaseWireInputOutputComponent
{
	[Sync]
	public float Length { get; set; } = 40;

	public override void WireInitialize()
	{
		this.RegisterInputHandler( "Length", ( float value ) =>
		{
			Length = value;
		}, Length );
	}
	public override PortType[] WireGetOutputs()
	{
		return new PortType[] {
			PortType.Float("Weight"),
		};
	}

	protected override void OnFixedUpdate()
	{
		if ( !this.IsValid() )
			return;

		var outputs = WirePorts.outputs;
		if ( !outputs.ContainsKey( "Weight" ) )
		{
			return;
		}

		var measuredWeight = 0f;

		// calculate weight of all props above this one
		var PhysicsBody = GetComponent<Rigidbody>().PhysicsBody;
		var Model = GetComponent<ModelCollider>().Model;
		var box = PhysicsBody.GetBounds().Grow( -4 ).Translate( WorldRotation.Up * Model.Bounds.Size.z );
		box = box.AddPoint( PhysicsBody.GetBounds().Center + WorldRotation.Up * (Length + Model.Bounds.Size.z / 2) );
		foreach ( GameObject ent in Scene.FindInPhysics( box ) )
		{
			if ( ent.GetComponent<PlayerController>() is PlayerController player )
			{
				// todo: is this right as of the new player controller?
				measuredWeight += 100 * player.GetComponent<Rigidbody>().PhysicsBody.GravityScale;
				continue;
			}
			if ( ent.GetComponent<Prop>() is not Prop prop )
				continue;
			if ( prop.GameObject == this.GameObject )
				continue;
			if ( prop.GetComponent<Rigidbody>().PhysicsBody is not PhysicsBody body )
				continue;
			if ( body.Velocity.z > 2 )
				continue;
			measuredWeight += body.Mass * body.GravityScale;
		}

		this.WireTriggerOutput( "Weight", measuredWeight );
	}
}
