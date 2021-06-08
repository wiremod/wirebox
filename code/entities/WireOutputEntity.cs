using System.Collections.Generic;
using System.Linq;

namespace Sandbox
{
	public class WireOutput
	{
		public int value;
		public Entity entity;
		public string outputName;
		public List<WireInput> connected = new List<WireInput>();

		public WireOutput( Entity entity, string newOutputName )
		{
			this.entity = entity;
			outputName = newOutputName;
		}
	}

	public interface WireOutputEntity : IWireEntity
	{
		public static void WireTriggerOutput( WireOutputEntity ent, string outputName, int value )
		{
			var output = ent.GetOutput( outputName );
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
			var connected = GetOutput( outputName ).connected;
			var input = inputEnt.GetInput( inputName );
			if ( !connected.Contains( input ) ) {
				connected.Add( input );
			}
		}

		public WireOutput GetOutput( string inputName )
		{
			if ( WirePorts.outputs.Count == 0 ) {
				InitializeOutputs();
			}
			return WirePorts.outputs[inputName];
		}
		public string[] GetOutputNames()
		{
			if ( WirePorts.outputs.Count == 0 ) {
				InitializeOutputs();
			}
			return WirePorts.outputs.Keys.ToArray();
		}

		protected void InitializeOutputs()
		{
			foreach ( var outputName in WireGetOutputs() ) {
				WirePorts.outputs[outputName] = new WireOutput( (Entity)this, outputName );
			}
		}
		abstract public string[] WireGetOutputs();
	}
}
