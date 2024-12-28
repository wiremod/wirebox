namespace Sandbox
{
	public class WireInput
	{
		public object value = 0;
		public float asFloat { get => Convert.ToSingle( value ); }
		public bool asBool { get => Convert.ToBoolean( value ); }
		public Vector3 asVector3
		{
			get
			{
				if ( value is Vector3 valueVec3 )
					return valueVec3;
				else if ( value is float valueFloat )
					return new Vector3( valueFloat, valueFloat, valueFloat );
				else if ( value is int valueInt )
					return new Vector3( valueInt, valueInt, valueInt );
				else if ( value is Angles valueAng )
					return valueAng.Forward;
				else if ( value is Rotation valueRot )
					return valueRot.Forward;
				else
					return Vector3.Zero;
			}
		}

		public GameObject entity;
		public string inputName;
		public string type;
		public WireOutput connectedOutput;
		public WireCable AttachRope { get; set; }

		public WireInput( GameObject entity, string inputName, string type, object def = null )
		{
			this.entity = entity;
			this.inputName = inputName;
			this.type = type;

			value = def ?? IWireComponent.GetDefaultValueFromType( type );
		}
	}

	public interface IWireInputComponent : IWireComponent
	{
		public void WireTriggerInput<T>( string inputName, T value )
		{
			if ( !WirePorts.inputsInitialized )
			{ // these get cleared by hot reloading
				WireInitialize();
				WirePorts.inputsInitialized = true;
			}
			WirePorts.inputs[inputName].value = value;
			WirePorts.inputHandlers[inputName]( value );
		}
		public abstract void WireInitialize();

		public WireInput GetInput( string inputName )
		{
			if ( !WirePorts.inputsInitialized )
			{
				WireInitialize();
				WirePorts.inputsInitialized = true;
			}
			return WirePorts.inputs[inputName];
		}
		public string[] GetInputNames( bool withValues = false )
		{
			if ( !WirePorts.inputsInitialized )
			{
				WireInitialize();
				WirePorts.inputsInitialized = true;
			}
			return !withValues
				? WirePorts.inputs.Keys.ToArray()
				: WirePorts.inputs.Keys.Select( ( string key ) =>
				{
					var type = WirePorts.inputs[key].type;
					if ( type == "string" )
						return $"{key} [{type}]: \"{WirePorts.inputs[key].value}\"";

					return $"{key} [{type}]: {WirePorts.inputs[key].value}";
				} ).ToArray();
		}

		public void DisconnectInput( string inputName )
		{
			DisconnectInput( GetInput( inputName ) );
		}
		public void DisconnectInput( WireInput input )
		{
			if ( input.connectedOutput != null )
			{
				input.connectedOutput.connected.Remove( input );
				input.connectedOutput = null;
			}
			if ( input.AttachRope != null )
			{
				input.AttachRope.Destroy();
				input.AttachRope = null;
			}

			WireTriggerInput( input.inputName, IWireComponent.GetDefaultValueFromType( input.type ) );
		}
	}
	public abstract class BaseWireInputComponent : BaseWireComponent, IWireInputComponent
	{
		public abstract void WireInitialize();
	}


	// Extension methods to allow calling the interface methods without explicit casting
	public static class BaseWireInputComponentUtils
	{
		public static void RegisterInputHandler( this IWireInputComponent instance, string inputName, Action<float> handler, float def = 0f )
		{
			instance.WirePorts.inputHandlers[inputName] = (( value ) =>
			{
				if ( value is bool valueBool )
				{
					handler( valueBool ? 1.0f : 0.0f );
				}
				else if ( value is Vector3 valueVec3 )
				{
					handler( valueVec3.Length );
				}
				else
				{
					handler( Convert.ToSingle( value ) );
				}
			});
			instance.WirePorts.inputs[inputName] = new WireInput( ((BaseWireComponent)instance).GameObject, inputName, "float", def );
		}

		public static void RegisterInputHandler( this IWireInputComponent instance, string inputName, Action<bool> handler, bool def = false )
		{
			instance.WirePorts.inputHandlers[inputName] = (( value ) =>
			{
				if ( value is int valueInt )
				{
					handler( valueInt != 0 );
				}
				else if ( value is float valueFloat )
				{
					handler( valueFloat.AlmostEqual( 0.0f ) );
				}
				else if ( value is Vector3 valueVec3 )
				{
					handler( valueVec3.IsNearZeroLength );
				}
				else
				{
					handler( (bool)value );
				}
			});
			instance.WirePorts.inputs[inputName] = new WireInput( ((BaseWireComponent)instance).GameObject, inputName, "bool", def );
		}

		public static void RegisterInputHandler( this IWireInputComponent instance, string inputName, Action<int> handler, int def = 0 )
		{
			instance.WirePorts.inputHandlers[inputName] = (( value ) =>
			{
				if ( value is Vector3 valueVec3 )
				{
					handler( (int)valueVec3.Length );
				}
				else
				{
					handler( (int)value );
				}
			});
			instance.WirePorts.inputs[inputName] = new WireInput( ((BaseWireComponent)instance).GameObject, inputName, "int", def );
		}

		public static void RegisterInputHandler( this IWireInputComponent instance, string inputName, Action<string> handler, string def = "" )
		{
			instance.WirePorts.inputHandlers[inputName] = (( value ) =>
			{
				if ( value is GameObject valueEnt )
				{
					handler( $"GameObject" ); // todo is there something more useful like $"{valueEnt.ClassName} [{valueEnt.NetworkIdent}]" we can do?
				}
				else if ( value is Vector3 valueVec3 )
				{
					handler( $"[{valueVec3.x:0.#},{valueVec3.y:0.#},{valueVec3.z:0.#}]" );
				}
				else if ( value is float valueFloat )
				{
					handler( valueFloat.ToString( "0.###" ) );
				}
				else
				{
					handler( value.ToString() );
				}
			});
			instance.WirePorts.inputs[inputName] = new WireInput( ((BaseWireComponent)instance).GameObject, inputName, "string", def );
		}

		public static void RegisterInputHandler( this IWireInputComponent instance, string inputName, Action<Vector3> handler, Vector3 def = new Vector3() )
		{
			instance.WirePorts.inputHandlers[inputName] = (( value ) =>
			{
				if ( value is Angles ang )
				{
					handler( ang.Forward );
				}
				else
				{
					handler( (Vector3)value );
				}
			});
			instance.WirePorts.inputs[inputName] = new WireInput( ((BaseWireComponent)instance).GameObject, inputName, "vector3", def );
		}

		public static void RegisterInputHandler( this IWireInputComponent instance, string inputName, Action<Angles> handler, Angles def = new Angles() )
		{
			instance.WirePorts.inputHandlers[inputName] = (( value ) =>
			{
				if ( value is Vector3 vec3 )
				{
					handler( vec3.EulerAngles );
				}
				else
				{
					handler( (Angles)value );
				}
			});
			instance.WirePorts.inputs[inputName] = new WireInput( ((BaseWireComponent)instance).GameObject, inputName, "angle", def );
		}

		// Todo: C# won't seem to let me make `def = Rotation.Identity`, so lets just have a second function for now
		public static void RegisterInputHandler( this IWireInputComponent instance, string inputName, Action<Rotation> handler )
		{
			RegisterInputHandler( instance, inputName, handler, Rotation.Identity );
		}

		public static void RegisterInputHandler( this IWireInputComponent instance, string inputName, Action<Rotation> handler, Rotation def )
		{
			instance.WirePorts.inputHandlers[inputName] = (( value ) =>
			{
				handler( (Rotation)value );
			});
			instance.WirePorts.inputs[inputName] = new WireInput( ((BaseWireComponent)instance).GameObject, inputName, "rotation", def );
		}

		public static void RegisterInputHandler( this IWireInputComponent instance, string inputName, Action<GameObject> handler, GameObject def = null )
		{
			instance.WirePorts.inputHandlers[inputName] = (( value ) =>
			{
				if ( value is GameObject valueGo )
				{
					handler( valueGo );
				}
				else
				{
					handler( null );
				}
			});
			instance.WirePorts.inputs[inputName] = new WireInput( ((BaseWireComponent)instance).GameObject, inputName, "gameobject", def );
		}

		public static void RegisterInputHandler( this IWireInputComponent instance, string inputName, Action<object> handler, object def = null )
		{
			instance.WirePorts.inputHandlers[inputName] = handler;
			instance.WirePorts.inputs[inputName] = new WireInput( ((BaseWireComponent)instance).GameObject, inputName, "any", def );
		}
	}

}
