using Sandbox;

public partial class ThrusterEntity : WireInputEntity
{
	// todo: the vanilla ThrusterEntity is just on/off, so we'll probably want to just replace it entirely, rather than this extending...
	[Net]
	public float ForceMultiplier { get; set; } = 1.0f;

	public string[] WireGetInputs()
	{
		return new string[] { "ForceMultiplier" };
	}

	void WireInputEntity.HandleWireInput<T>( string inputName, T value )
	{
		if ( inputName == "ForceMultiplier" ) {
			if ( typeof( T ) == typeof( int ) ) {
				ForceMultiplier = ((int)(object)value);
			}
			if ( typeof( T ) == typeof( float ) ) {
				ForceMultiplier = (float)(object)value;
			}
			Enabled = ForceMultiplier != 0.0f;
		}
	}
}
