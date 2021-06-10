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

		public void RegisterInputHandler( string inputName, Action<bool> handler )
		{
			WirePorts.inputHandlers[inputName] = (( value ) => {
				if ( value is bool valueBool ) {
					handler( valueBool );
				}
				else {
					handler( ((int)value)!=0 );
				}
			});
			WirePorts.inputs[inputName] = new WireInput( (Entity)this, inputName, "bool" );
		}
		public void RegisterInputHandler( string inputName, Action<float> handler )
		{
			WirePorts.inputHandlers[inputName] = (( value ) => {
				handler( (float)value );
			});
			WirePorts.inputs[inputName] = new WireInput( (Entity)this, inputName, "float" );
		}
		public void RegisterInputHandler( string inputName, Action<int> handler )
		{
			WirePorts.inputHandlers[inputName] = (( value ) => {
				handler( (int)value );
			});
			WirePorts.inputs[inputName] = new WireInput( (Entity)this, inputName, "int" );
		}
	}

}
