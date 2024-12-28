namespace Sandbox
{
	public class WirePortData
	{
		public bool inputsInitialized = false;
		public Dictionary<string, Action<object>> inputHandlers { get; } = [];
		public Dictionary<string, WireInput> inputs = [];
		public Dictionary<string, WireOutput> outputs = [];
	}

	public interface IWireComponent
	{
		public WirePortData WirePorts { get; }

		public virtual string GetOverlayText() { return ""; }

		public static object GetDefaultValueFromType( string type )
		{
			if ( type == "bool" )
				return false;
			else if ( type == "int" )
				return 0;
			else if ( type == "float" )
				return 0.0f;
			else if ( type == "string" )
				return "";
			else if ( type == "vector3" )
				return Vector3.Zero;
			else if ( type == "angle" )
				return Angles.Zero;
			else if ( type == "rotation" )
				return Rotation.Identity;
			else if ( type == "gameobject" )
			{
				return 0; // this... isn't great, but null's are worse (eg. TriggerOutput's `output.value.Equals( value )` check errors)
			}

			return false;
		}
	}
	public class BaseWireComponent : Component, IWireComponent
	{
		public WirePortData WirePorts { get; } = new();
	}
}
