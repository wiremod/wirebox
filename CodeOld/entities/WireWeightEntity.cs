using System;
using Sandbox;

[Library( "ent_wireweight", Title = "Wire Weight" )]
public partial class WireWeightEntity : Prop, IWireInputEntity
{
	[Net]
	public float Weight { get; set; } = 100;

	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();
	public void WireInitialize()
	{
		this.RegisterInputHandler( "Weight", ( float val ) =>
		{
			UpdateMass( val );
		}, Weight );
		UpdateMass( Weight );
	}

	private void UpdateMass( float val )
	{
		if ( val.AlmostEqual( 0 ) )
		{
			val = 0.1f;
		}
		Weight = val;
		PhysicsBody.Mass = Math.Abs( Weight );
		PhysicsBody.GravityScale = Math.Abs( PhysicsBody.GravityScale ) * (Weight < 0 ? -1f : 1f);
		PhysicsBody.Sleeping = false;
	}
}
