using System.Collections.Generic;
using System.Linq;
using System;

namespace Sandbox
{
	public class WireInput
	{
		public int value;
		public Entity entity;
		public string inputName;
		public string type;
		public WireOutput connectedOutput;

		public WireInput( Entity entity, string inputName, string type )
		{
			this.entity = entity;
			this.inputName = inputName;
			this.type = type;
		}
	}
	public interface WireInputEntity : IWireEntity
	{
		public void WireTriggerInput<T>( string inputName, T value )
		{
			if ( WirePorts.inputHandlers.Count == 0 ) { // these get cleared by hot reloading
				WireInitialize();
			}
			WirePorts.inputHandlers[inputName]( value );
		}
		public virtual void WireInitialize() { }

		public WireInput GetInput( string inputName )
		{
			if ( WirePorts.inputHandlers.Count == 0 ) {
				WireInitialize();
			}
			return WirePorts.inputs[inputName];
		}
		public string[] GetInputNames()
		{
			if ( WirePorts.inputHandlers.Count == 0 ) {
				WireInitialize();
			}
			return WirePorts.inputs.Keys.ToArray();
		}

		public void DisconnectInput(string inputName)
		{
			var input = GetInput(inputName);
			if (input.connectedOutput != null) {
				input.connectedOutput.connected.Remove(input);
				input.connectedOutput = null;
			}
		}

		public void RegisterInputHandler<T>( string inputName, Action<T> handler )
		{
			if ( typeof( T ) == typeof( bool ) ) {
				WirePorts.inputHandlers[inputName] = (( value ) => {
					if ( value is int valueInt ) {
						handler( (T)(object)(valueInt != 0) );
					}
					else if ( value is float valueFloat ) {
						handler( (T)(object)(valueFloat != 0.0f) );
					}
					else {
						handler( (T)value );
					}
				});
				WirePorts.inputs[inputName] = new WireInput( (Entity)this, inputName, "bool" );
			}
			else if ( typeof( T ) == typeof( float ) ) {
				WirePorts.inputHandlers[inputName] = (( value ) => {
					if ( value is int valueInt ) {
						handler( (T)(object)(float)(valueInt) );
					}
					else {
						handler( (T)value );
					}
				});
				WirePorts.inputs[inputName] = new WireInput( (Entity)this, inputName, "float" );
			}
			else if ( typeof( T ) == typeof( int ) ) {
				WirePorts.inputHandlers[inputName] = (( value ) => {
					if ( value is float valueInt ) {
						handler( (T)(object)(int)(valueInt) );
					}
					else {
						handler( (T)value );
					}
				});
				WirePorts.inputs[inputName] = new WireInput( (Entity)this, inputName, "int" );
			}
			else {
				throw new Exception( "Wirebox RegisterInputHandler<" + typeof( T ) + "> unhandled type for " + this.GetType() );
			}
		}
	}

}
