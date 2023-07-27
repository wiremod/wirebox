using Sandbox;
/* Todo 2023: partial classes don't stretch between assemblies, so this won't work as of being an addon
public partial class LightEntity : WireInputEntity
{
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();
	public void WireInitialize()
	{
		this.RegisterInputHandler( "On", ( bool value ) => {
			Enabled = value;
		} );
		this.RegisterInputHandler( "Brightness", ( float value ) => {
			BrightnessMultiplier = value;
		} );
	}
}
*/
