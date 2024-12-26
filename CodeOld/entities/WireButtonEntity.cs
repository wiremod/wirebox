using Sandbox;
using System;
using System.Linq;

[Spawnable]
[Library( "ent_wirebutton", Title = "Wire Button" )]
public partial class WireButtonEntity : Prop, IUse, IStopUsing, IWireOutputEntity
{
	public bool On { get; set; } = false;
	public bool IsToggle { get; set; } = false;
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();

	public override void Spawn()
	{
		base.Spawn();
		SetModel( "models/wirebox/katlatze/button.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
	}
	public bool IsUsable( Entity user )
	{
		return true;
	}

	public bool OnUse( Entity user )
	{
		if ( IsToggle )
		{
			SetOn( !On );
			return false;
		}
		else
		{
			SetOn( true );
			return true;
		}
	}

	public void OnStopUsing( Entity user )
	{
		if ( !IsToggle )
		{
			SetOn( false );
		}
	}

	protected void SetOn( bool on )
	{
		On = on;
		if ( MaterialGroupCount > 1 )
		{
			SetMaterialGroup( On ? 1 : 0 );
		}
		else
		{
			RenderColor = On ? Color.Green : Color.Red;
		}
		this.WireTriggerOutput( "On", On );
	}


	public PortType[] WireGetOutputs()
	{
		return new PortType[] { PortType.Bool( "On" ) };
	}
}

