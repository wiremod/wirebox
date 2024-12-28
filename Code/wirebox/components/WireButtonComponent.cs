[Library( "ent_wirebutton", Title = "Wire Button" )]
public partial class WireButtonComponent : BaseWireOutputComponent, Component.IPressable
{
	public bool On { get; set; } = false;
	public bool IsToggle { get; set; } = false;
	protected Connection pressedConnection;

	protected override void OnUpdate()
	{
		// workaround for IPressable.Release not working
		if ( Connection.Local == pressedConnection )
		{
			if ( !Input.Down( "use" ) )
			{
				SetOn( false );
				pressedConnection = null;
			}
		}

	}
	bool IPressable.CanPress( IPressable.Event e )
	{
		return true;
	}
	bool IPressable.Press( IPressable.Event e )
	{
		SetOn( !On );
		if ( !IsToggle )
		{
			pressedConnection = e.Source.Network.Owner;
		}
		return true;
	}
	void IPressable.Release( IPressable.Event e )
	{
		Log.Info( "IPressable.Release is working if you can see this" ); // doesn't seem to be working right now
		if ( !IsToggle )
		{
			SetOn( false );
		}
	}

	[Rpc.Broadcast]
	protected void SetOn( bool on )
	{
		On = on;
		var renderer = GetComponent<ModelRenderer>();
		if ( renderer.Model.MaterialGroupCount > 1 )
		{
			renderer.MaterialGroup = renderer.Model.GetMaterialGroupName( On ? 1 : 0 );
		}
		else
		{
			renderer.Tint = On ? Color.Green : Color.Red;
		}
		this.WireTriggerOutput("On", On);
		Sound.Play( On ? "flashlight-on" : "flashlight-off", WorldPosition );
	}

	public override PortType[] WireGetOutputs()
	{
		return [PortType.Bool( "On" )];
	}
}

