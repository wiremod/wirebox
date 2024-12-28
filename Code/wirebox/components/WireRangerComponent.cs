[Library( "ent_wireranger", Title = "Wire Ranger" )]
public partial class WireRangerComponent : BaseWireInputOutputComponent
{
	[Sync]
	public float Length { get; set; } = 100;
	[Sync]
	public bool HitWater { get; set; } = false;
	[Sync]
	public bool HitWorld { get; set; } = true;
	[Sync]
	public bool ShowBeam { get; set; } = true;
	public bool DefaultZero { get; set; } = true;
	private LegacyParticleSystem Beam;
	private SceneTraceResult lastTrace;
	public override void WireInitialize()
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
	public override PortType[] WireGetOutputs()
	{
		return new PortType[] {
			PortType.Float("Distance"),
			PortType.Float("X"),
			PortType.Float("Y"),
			PortType.Float("Z"),
			PortType.Vector3("Position"),
			PortType.Vector3("Normal"),
			// PortType.GameObject("GameObject"),
			// PortType.Int("EntityID"),
		};
	}

	protected override void OnFixedUpdate()
	{
		if ( !this.IsValid() )
		{
			return;
		}

		var tr = DoTrace();
		lastTrace = tr;

		var outputs = WirePorts.outputs;
		if ( !outputs.ContainsKey( "Distance" ) )
		{
			return;
		}

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
				// this.WireTriggerOutput( "GameObject", (GameObject)null ); // seems to error
				// this.WireTriggerOutput( "EntityID", 0 );
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
				// this.WireTriggerOutput( "GameObject", tr.GameObject );
				// this.WireTriggerOutput( "EntityID", tr.Entity?.NetworkIdent );
			}
		}
	}

	private SceneTraceResult DoTrace()
	{
		var Offset = WorldRotation.Up * GetComponent<Rigidbody>().PhysicsBody.GetBounds().Size.z;
		var trace = Scene.Trace.Ray( WorldPosition + Offset, WorldPosition + Offset + WorldRotation.Up * Length )
			.UseHitboxes()
			.WithAnyTags( "solid", "npc", "glass" )
			.WithoutTags( "debris", "player" )
			.IgnoreGameObjectHierarchy( GameObject )
			.Size( 2 );

		if ( HitWater )
		{
			trace.WithoutTags( "water" );
		}
		if ( !HitWorld )
		{
			trace.IgnoreStatic();
		}

		return trace.Run();
	}

	protected override void OnUpdate()
	{
		if ( !ShowBeam )
		{
			if ( Beam != null )
			{
				Beam.Destroy();
				Beam = null;
			}
			return;
		}
		if ( Beam == null )
		{
			Beam = Particles.MakeParticleSystem( "particles/wirebox/ranger_beam.vpcf", Transform.World, 0, GameObject );
		}
		Beam.SceneObject?.SetControlPoint( 1, lastTrace.EndPosition );
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		Beam?.Destroy();
	}
}
