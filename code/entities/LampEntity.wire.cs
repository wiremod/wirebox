using Sandbox;

public partial class LampEntity : WireInputEntity
{
	public string[] WireGetInputs()
	{
		return new string[] { "On" };
	}

	void WireInputEntity.HandleWireInput<T>( string inputName, T value )
	{
		if ( inputName == "On" ) {
			if ( typeof( T ) == typeof( int ) ) {
				Enabled = ((int)(object)value) != 0;
			}
			if ( typeof( T ) == typeof( float ) ) {
				Enabled = (float)(object)value != 0.0f;
			}
		}
	}
}
