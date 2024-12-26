using Sandbox;

[Library( "ent_wireforcer", Title = "Wire Forcer" )]
public partial class WireForcerEntity : Prop, IWireInputEntity
{
	[Net]
	public float Length { get; set; } = 100;
	[Net]
	public float Force { get; set; } = 0;
	[Net]
	public float OffsetForce { get; set; } = 0;
	[Net]
	public bool ShowBeam { get; set; } = true;
	[Net]
	public bool IgnoreMass { get; set; } = true;

	private Particles Beam;
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();
	public void WireInitialize()
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

	[GameEvent.Physics.PostStep]
	public void OnPostPhysicsStep()
	{
		if ( !this.IsValid() )
			return;

		var tr = DoTrace();

		if ( tr.Hit && tr.Body.IsValid() )
		{
			if ( Force != 0 )
			{
				var appliedForce = Rotation.Up * Force;
				if ( IgnoreMass )
				{
					appliedForce *= tr.Body.Mass;
				}
				tr.Body.ApplyForce( appliedForce );
			}
			if ( OffsetForce != 0 )
			{
				var appliedForce = Rotation.Up * OffsetForce;
				if ( IgnoreMass )
				{
					appliedForce *= tr.Body.Mass;
				}
				tr.Body.ApplyForceAt( tr.HitPosition, appliedForce );
			}
		}
	}

	private TraceResult DoTrace()
	{
		var Offset = Rotation.Up * CollisionBounds.Size.z;
		var trace = Trace.Ray( Position + Offset, Position + Offset + Rotation.Up * Length )
			.DynamicOnly()
			.Ignore( this );

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
