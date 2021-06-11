using Sandbox;

public partial class LampEntity : WireInputEntity
{
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();
	public void WireInitialize()
	{
		this.RegisterInputHandler( "On", ( bool value ) => {
			Enabled = value;
		} );
	}
}
