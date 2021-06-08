using Sandbox;
public partial class LightEntity : WireInputEntity
{
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();
	public void WireInitialize()
	{
		((WireInputEntity)this).RegisterInputHandler( "On", ( bool value ) => {
			Enabled = value;
		} );
	}
}
