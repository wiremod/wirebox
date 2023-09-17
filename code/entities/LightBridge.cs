using Sandbox;

[Spawnable]
[Library( "ent_wirelightbridge", Title = "Light Bridge" )]
public partial class LightBridgeEntity : Prop, IWireInputEntity
{
	private MeshEntity bridgeEntity;
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();

	public void WireInitialize()
	{
		this.RegisterInputHandler( "Length", ( float length ) =>
		{
			if ( length < 10 )
			{
				bridgeEntity?.Delete();
				return;
			}
			var vertexModel = VertexMeshBuilder.CreateRectangle( (int)length, 100, 1, 64 );
			if ( !bridgeEntity.IsValid() )
			{
				bridgeEntity = VertexMeshBuilder.SpawnEntity( vertexModel );
				bridgeEntity.Position = Transform.PointToWorld( new Vector3( 4, -50, 9.5f ) - bridgeEntity.CollisionBounds.Mins );
				bridgeEntity.Rotation = Rotation;
				bridgeEntity.MaterialOverride = "materials/wirebox/katlatze/metal.vmat";
				bridgeEntity.RenderColor = new Color( 0, 90, 255, 180 );
				this.Weld( bridgeEntity );
			}
			else
			{
				bridgeEntity.Model = VertexMeshBuilder.Models[vertexModel];
				bridgeEntity.Tick();
				bridgeEntity.Position = Transform.PointToWorld( new Vector3( 4, -50, 9.5f ) - bridgeEntity.CollisionBounds.Mins );
			}
		} );
	}

	protected override void OnDestroy()
	{
		if ( bridgeEntity.IsValid() )
		{
			bridgeEntity.Delete();
		}
		base.OnDestroy();
	}

	public static void CreateFromTool( Player owner, TraceResult tr )
	{
		var ent = new LightBridgeEntity
		{
			Position = tr.EndPosition,
			Rotation = Rotation.LookAt( tr.Normal, owner.EyeRotation.Forward ) * Rotation.From( new Angles( 90, 0, 0 ) ),
		};

		if ( tr.Body.IsValid() && !tr.Entity.IsWorld )
		{
			ent.SetParent( tr.Body.GetEntity(), tr.Body.GroupName );
		}

		ent.SetModel( "models/wirebox/katlatze/lightbridge.vmdl" );

		Event.Run( "entity.spawned", ent, owner );
	}
	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/wirebox/katlatze/lightbridge.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
	}
}

