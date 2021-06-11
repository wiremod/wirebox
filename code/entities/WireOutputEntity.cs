using System.Collections.Generic;
using System.Linq;

namespace Sandbox
{
	public class WireOutput
	{
		public object value = 0;
		public Entity entity;
		public string outputName;
		public List<WireInput> connected = new();

		public WireOutput( Entity entity, string newOutputName )
		{
			this.entity = entity;
			outputName = newOutputName;
		}
	}

	public interface WireOutputEntity : IWireEntity
	{
		public void WireTriggerOutput<T>( string outputName, T value )
		{
			if ( WirePorts.outputExecutionsTick != Time.Tick ) {
				WirePorts.outputExecutionsTick = Time.Tick;
				WirePorts.outputExecutions = 0;
			}
			if ( WirePorts.outputExecutions >= 4 ) {
				// prevent infinite loops
				return; // todo: queue for next tick?
			}
			WirePorts.outputExecutions++;

			var output = GetOutput( outputName );
			output.value = value;
			foreach ( var input in output.connected ) {
				if ( !input.entity.IsValid() ) continue;
				if ( input.entity is WireInputEntity inputEntity ) {
					inputEntity.WireTriggerInput( input.inputName, value );
				}
			}
		}
		public void WireConnect( WireInputEntity inputEnt, string outputName, string inputName )
		{
			var input = inputEnt.GetInput( inputName );
			var output = GetOutput( outputName );
			var connected = output.connected;
			if ( !connected.Contains( input ) ) {
				if ( input.connectedOutput != null ) {
					inputEnt.DisconnectInput( inputName );
				}
				input.connectedOutput = output;
				connected.Add( input );
			}
		}

		public WireOutput GetOutput( string inputName )
		{
			if ( WirePorts.outputs.Count == 0 ) {
				WireInitializeOutputs();
			}
			return WirePorts.outputs[inputName];
		}
		public string[] GetOutputNames()
		{
			if ( WirePorts.outputs.Count == 0 ) {
				WireInitializeOutputs();
			}
			return WirePorts.outputs.Keys.ToArray();
		}

		// A thin wrapper, so classes can replaces this as needed
		public virtual void WireInitializeOutputs()
		{
			InitializeOutputs();
		}
		public void InitializeOutputs()
		{
			foreach ( var outputName in WireGetOutputs() ) {
				WirePorts.outputs[outputName] = new WireOutput( (Entity)this, outputName );
			}
		}
		abstract public string[] WireGetOutputs();
	}

	// Extension methods to allow calling the interface methods without explicit casting
	public static class WireOutputEntityUtils
	{
		public static void WireTriggerOutput<T>( this WireOutputEntity instance, string outputName, T value )
		{
			instance.WireTriggerOutput( outputName, value );
		}
	}

}
