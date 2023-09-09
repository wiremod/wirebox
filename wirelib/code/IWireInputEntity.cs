using System.Collections.Generic;
using System.Linq;
using System;

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

		public Entity entity;
		public string inputName;
		public string type;
		public WireOutput connectedOutput;
		public WireCable AttachRope { get; set; }

		public WireInput( Entity entity, string inputName, string type, object def = null )
		{
			this.entity = entity;
			this.inputName = inputName;
			this.type = type;

			value = def ?? IWireEntity.GetDefaultValueFromType( type );
		}
	}

	public interface IWireInputEntity : IWireEntity
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
		public virtual void WireInitialize() { }

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
				input.AttachRope.Destroy( true );
				input.AttachRope = null;
			}

			WireTriggerInput( input.inputName, IWireEntity.GetDefaultValueFromType( input.type ) );
		}

	}

	// Extension methods to allow calling the interface methods without explicit casting
	public static class IWireInputEntityUtils
	{
		public static void RegisterInputHandler( this IWireInputEntity instance, string inputName, Action<float> handler, float def = 0f )
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
			instance.WirePorts.inputs[inputName] = new WireInput( (Entity)instance, inputName, "float", def );
		}

		public static void RegisterInputHandler( this IWireInputEntity instance, string inputName, Action<bool> handler, bool def = false )
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
			instance.WirePorts.inputs[inputName] = new WireInput( (Entity)instance, inputName, "bool", def );
		}

		public static void RegisterInputHandler( this IWireInputEntity instance, string inputName, Action<int> handler, int def = 0 )
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
			instance.WirePorts.inputs[inputName] = new WireInput( (Entity)instance, inputName, "int", def );
		}

		public static void RegisterInputHandler( this IWireInputEntity instance, string inputName, Action<string> handler, string def = "" )
		{
			instance.WirePorts.inputHandlers[inputName] = (( value ) =>
			{
				if ( value is Entity valueEnt )
				{
					handler( $"{valueEnt.ClassName} [{valueEnt.NetworkIdent}]" );
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
			instance.WirePorts.inputs[inputName] = new WireInput( (Entity)instance, inputName, "string", def );
		}

		public static void RegisterInputHandler( this IWireInputEntity instance, string inputName, Action<Vector3> handler, Vector3 def = new Vector3() )
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
			instance.WirePorts.inputs[inputName] = new WireInput( (Entity)instance, inputName, "vector3", def );
		}

		public static void RegisterInputHandler( this IWireInputEntity instance, string inputName, Action<Angles> handler, Angles def = new Angles() )
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
			instance.WirePorts.inputs[inputName] = new WireInput( (Entity)instance, inputName, "angle", def );
		}

		// Todo: C# won't seem to let me make `def = Rotation.Identity`, so lets just have a second function for now
		public static void RegisterInputHandler( this IWireInputEntity instance, string inputName, Action<Rotation> handler )
		{
			RegisterInputHandler( instance, inputName, handler, Rotation.Identity );
		}

		public static void RegisterInputHandler( this IWireInputEntity instance, string inputName, Action<Rotation> handler, Rotation def )
		{
			instance.WirePorts.inputHandlers[inputName] = (( value ) =>
			{
				handler( (Rotation)value );
			});
			instance.WirePorts.inputs[inputName] = new WireInput( (Entity)instance, inputName, "rotation", def );
		}

		public static void RegisterInputHandler( this IWireInputEntity instance, string inputName, Action<Entity> handler, Entity def = null )
		{
			instance.WirePorts.inputHandlers[inputName] = (( value ) =>
			{
				if ( value is Entity valueEnt )
				{
					handler( (Entity)value );
				}
				else
				{
					handler( null );
				}
			});
			instance.WirePorts.inputs[inputName] = new WireInput( (Entity)instance, inputName, "entity", def );
		}

		public static void RegisterInputHandler( this IWireInputEntity instance, string inputName, Action<object> handler, object def = null )
		{
			instance.WirePorts.inputHandlers[inputName] = handler;
			instance.WirePorts.inputs[inputName] = new WireInput( (Entity)instance, inputName, "any", def );
		}
	}

}
