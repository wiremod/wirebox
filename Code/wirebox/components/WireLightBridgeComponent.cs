namespace Wirebox.Components;
[Spawnable]
[Library( "ent_wirelightbridge", Title = "Light Bridge" )]
public partial class WireLightBridgeComponent : BaseWireInputComponent
{
	private GameObject bridgeEntity;
	private float Length = 0;

	public override void WireInitialize()
	{
		this.RegisterInputHandler( "Length", ( float length ) =>
		{
			length = MathF.Round( length, 1 ); // precision of 0.1
			if ( Length.AlmostEqual( length, 0.5f ) ) // except lets not actually update the model for that minor a change
			{
				return;
			}
			if ( length < 10 )
			{
				bridgeEntity?.Destroy();
				return;
			}
			Length = length;
			var vertexModel = VertexMeshBuilder.CreateRectangle( (int)length, 100, 1, 64 );
			if ( !bridgeEntity.IsValid() )
			{
				bridgeEntity = VertexMeshBuilder.SpawnEntity( vertexModel );
			}
			else
			{
				bridgeEntity.GetComponent<PropHelper>().RemoveConstraints( ConstraintType.Weld, this.GameObject ); // the weld becomes invalid once we update the model
				var bridgeProp = bridgeEntity.GetComponent<Prop>();
				bridgeProp.Model = VertexMeshBuilder.Models[vertexModel];
			}
			var bridgeRenderer = bridgeEntity.GetComponent<ModelRenderer>();
			bridgeRenderer.SetMaterialOverride( Material.Load( "materials/wirebox/katlatze/metal.vmat" ), "" );
			bridgeRenderer.Tint = new Color( 0, 0.35f, 1, 0.7f );
			bridgeEntity.WorldPosition = Transform.World.PointToWorld( new Vector3( 4, -50, 9.5f ) - bridgeRenderer.Model.PhysicsBounds.Mins );
			bridgeEntity.WorldRotation = WorldRotation;
			var bridgePropHelper = bridgeEntity.GetComponent<PropHelper>();
			if ( bridgePropHelper.Rigidbody.Mass < 100 )
				bridgePropHelper.Rigidbody.MassOverride = 100; // to make it nicer to walk on
			bridgePropHelper.Weld( this.GameObject );
			
		} );
	}

	protected override void OnDestroy()
	{
		bridgeEntity?.Destroy();
		base.OnDestroy();
	}
}
