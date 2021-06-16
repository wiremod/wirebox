using Sandbox;
using System;
using System.Linq;

[Library( "ent_button", Title = "Button", Spawnable = true )]
public partial class ButtonEntity : Prop, IUse, IStopUsing, WireOutputEntity
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

	private bool ModelUsesMaterialGroups()
	{
		var model = GetModelName();
		if ( model == "models/wirebox/katlatze/button.vmdl" ) {
			return true;
		}
		return false;
	}

	public bool OnUse( Entity user )
	{
		if ( IsToggle ) {
			SetOn( !On );
			return false;
		}
		else {
			SetOn( true );
			return true;
		}
	}

	public void OnStopUsing( Entity user )
	{
		if ( !IsToggle ) {
			SetOn( false );
		}
	}

	protected void SetOn( bool on )
	{
		On = on;
		if ( ModelUsesMaterialGroups() ) {
			SetMaterialGroup( On ? 1 : 0 );
		}
		else {
			RenderColor = On ? Color32.Green : Color32.Red;
		}
		this.WireTriggerOutput( "On", On ? 1 : 0 );
	}


	public string[] WireGetOutputs()
	{
		return new string[] { "On" };
	}
}

