using Sandbox;

[Library( "ent_wireweightscale", Title = "Wire Weight Scale" )]
public partial class WireWeightScaleEntity : Prop, IWireInputEntity, IWireOutputEntity
{
	[Net]
	public float Length { get; set; } = 40;

	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();
	public void WireInitialize()
	{
		this.RegisterInputHandler( "Length", ( float value ) =>
		{
			Length = value;
		}, Length );
	}
	public PortType[] WireGetOutputs()
	{
		return new PortType[] {
			PortType.Float("Weight"),
		};
	}

	[GameEvent.Physics.PostStep]
	public void OnPostPhysicsStep()
	{
		if ( Game.IsClient )
			return;
		if ( !this.IsValid() )
			return;

		var outputs = ((IWireEntity)this).WirePorts.outputs;
		if ( !outputs.ContainsKey( "Weight" ) )
		{
			return;
		}

		var measuredWeight = 0f;

		// calculate weight of all props above this one
		var box = PhysicsBody.GetBounds() + Transform.Rotation.Up * Model.Bounds.Size.z;
		box = box.AddPoint( PhysicsBody.GetBounds().Center + Transform.Rotation.Up * (Length + Model.Bounds.Size.z / 2) );
		foreach ( Entity ent in Entity.FindInBox( box ) )
		{
			if ( ent is Player player )
			{
				measuredWeight += 100 * player.PhysicsBody.GravityScale;
				continue;
			}
			if ( ent is not Prop prop )
				continue;
			if ( prop == this )
				continue;
			if ( prop.PhysicsBody == null )
				continue;
			if ( prop.Velocity.z > 2 )
				continue;
			measuredWeight += prop.PhysicsBody.Mass * prop.PhysicsBody.GravityScale;
		}

		this.WireTriggerOutput( "Weight", measuredWeight );
	}
}
