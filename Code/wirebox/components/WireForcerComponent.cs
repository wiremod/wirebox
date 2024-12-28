using Sandbox;

[Library( "ent_wireforcer", Title = "Wire Forcer" )]
public partial class WireForcerComponent : BaseWireInputComponent
{
	[Sync]
	public float Length { get; set; } = 100;
	[Sync]
	public float Force { get; set; } = 0;
	[Sync]
	public float OffsetForce { get; set; } = 0;
	[Sync]
	public bool ShowBeam { get; set; } = true;
	[Sync]
	public bool IgnoreMass { get; set; } = true;

	private LegacyParticleSystem Beam;
	private SceneTraceResult lastTrace;
	public override void WireInitialize()
	{
		this.RegisterInputHandler( "Length", ( float value ) =>
		{
			Length = value;
		}, Length );

		this.RegisterInputHandler( "Force", ( float value ) =>
		{
			Force = value;
		}, Force );

		this.RegisterInputHandler( "OffsetForce", ( float value ) =>
		{
			OffsetForce = value;
		}, OffsetForce );
	}

	protected override void OnFixedUpdate()
	{
		if ( !this.IsValid() )
			return;

		var tr = DoTrace();
		lastTrace = tr;

		if ( tr.Hit && tr.Body.IsValid() )
		{
			if ( Force != 0 )
			{
				var appliedForce = WorldRotation.Up * Force;
				if ( IgnoreMass )
				{
					appliedForce *= tr.Body.Mass;
				}
				tr.Body.ApplyForce( appliedForce );
			}
			if ( OffsetForce != 0 )
			{
				var appliedForce = WorldRotation.Up * OffsetForce;
				if ( IgnoreMass )
				{
					appliedForce *= tr.Body.Mass;
				}
				tr.Body.ApplyForceAt( tr.HitPosition, appliedForce );
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
