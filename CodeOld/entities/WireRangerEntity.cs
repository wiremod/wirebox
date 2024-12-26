using Sandbox;

[Library( "ent_wireranger", Title = "Wire Ranger" )]
public partial class WireRangerEntity : Prop, IWireOutputEntity, IWireInputEntity
{
	[Net]
	public float Length { get; set; } = 100;
	[Net]
	public bool HitWater { get; set; } = false;
	[Net]
	public bool HitWorld { get; set; } = true;
	[Net]
	public bool ShowBeam { get; set; } = true;
	public bool DefaultZero { get; set; } = true;
	private Particles Beam;
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();
	public void WireInitialize()
	{
		this.RegisterInputHandler( "Length", ( float value ) =>
		{
			Length = value;
		}, Length );

		this.RegisterInputHandler( "HitWater", ( bool value ) =>
		{
			HitWater = value;
		}, HitWater );

		this.RegisterInputHandler( "HitWorld", ( bool value ) =>
		{
			HitWorld = value;
		}, HitWorld );

		this.RegisterInputHandler( "ShowBeam", ( bool value ) =>
		{
			ShowBeam = value;
		}, ShowBeam );

		this.RegisterInputHandler( "DefaultZero", ( bool value ) =>
		{
			DefaultZero = value;
		}, DefaultZero );
	}
	public PortType[] WireGetOutputs()
	{
		return new PortType[] {
			PortType.Float("Distance"),
			PortType.Float("X"),
			PortType.Float("Y"),
			PortType.Float("Z"),
			PortType.Vector3("Position"),
			PortType.Vector3("Normal"),
			// PortType.Entity("Entity"),
			PortType.Int("EntityID"),
		};
	}

	[Event.Physics.PostStep]
	public void OnPostPhysicsStep()
	{
		if ( !this.IsValid() )
			return;

		var outputs = ((IWireEntity)this).WirePorts.outputs;
		if ( !outputs.ContainsKey( "Distance" ) )
		{
			return;
		}
		var tr = DoTrace();

		if ( !tr.Hit && DefaultZero )
		{
			if ( outputs["Distance"].value is not float oldDistance || oldDistance != 0 )
			{
				this.WireTriggerOutput( "Distance", 0 );
				this.WireTriggerOutput( "X", 0 );
				this.WireTriggerOutput( "Y", 0 );
				this.WireTriggerOutput( "Z", 0 );
				this.WireTriggerOutput( "Position", Vector3.Zero );
				this.WireTriggerOutput( "Normal", Vector3.Zero );
				this.WireTriggerOutput( "EntityID", 0 );
			}
		}
		else
		{
			var newDistance = tr.Hit ? tr.Distance : Length;
			if ( outputs["Distance"].value is not float oldDistance || oldDistance != newDistance )
			{
				this.WireTriggerOutput( "Distance", newDistance );
			}
			if ( outputs["Position"].value is not Vector3 oldValue || !oldValue.Equals( tr.EndPosition ) )
			{
				this.WireTriggerOutput( "X", tr.EndPosition.x );
				this.WireTriggerOutput( "Y", tr.EndPosition.y );
				this.WireTriggerOutput( "Z", tr.EndPosition.z );
				this.WireTriggerOutput( "Position", tr.EndPosition );
				this.WireTriggerOutput( "Normal", tr.Normal );
				this.WireTriggerOutput( "EntityID", tr.Entity?.NetworkIdent );
			}
		}
	}

	private TraceResult DoTrace()
	{
		var Offset = Rotation.Up * CollisionBounds.Size.z;
		var trace = Trace.Ray( Position + Offset, Position + Offset + Rotation.Up * Length )
			.Ignore( this );

		if ( HitWater )
		{
			trace.WithoutTags( "water" );
		}

		if ( !HitWorld )
		{
			trace.DynamicOnly();
		}

		return trace.Run();
	}

	[GameEvent.Client.Frame]
	public void OnFrame()
	{
		if ( !ShowBeam )
		{
			if ( Beam != null )
			{
				Beam.Destroy( true );
				Beam = null;
			}
			return;
		}
		var tr = DoTrace();
		if ( Beam == null )
		{
			Beam = Particles.Create( "particles/wirebox/ranger_beam.vpcf", this, "root" );
		}
		Beam.SetPosition( 1, tr.EndPosition );
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		Beam?.Destroy( true );
	}
}
