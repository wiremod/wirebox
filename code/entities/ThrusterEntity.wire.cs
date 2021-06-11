using Sandbox;

public partial class ThrusterEntity : WireInputEntity
{
	// todo: the vanilla ThrusterEntity is just on/off, so we'll probably want to just replace it entirely, rather than this extending...
	[Net]
	public float ForceMultiplier { get; set; } = 1.0f;
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();
	public void WireInitialize()
	{
		this.RegisterInputHandler( "ForceMultiplier", ( float value ) => {
			ForceMultiplier = value;
			Enabled = ForceMultiplier != 0.0f;
		} );
	}
}
