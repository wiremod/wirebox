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

	public interface WireOutputEntity
	{
		protected static Dictionary<int, Dictionary<string, WireOutput>> allOutputs = new Dictionary<int, Dictionary<string, WireOutput>>();

		public int NetworkIdent { get; }

		public static void WireTriggerOutput( WireOutputEntity ent, string outputName, int value )
		{
			var output = ent.GetOutput( outputName );
			foreach ( var input in output.connected ) {
				if ( !input.entity.IsValid() ) continue;
				if ( input.entity is WireInputEntity inputEntity ) {
					inputEntity.WireTriggerInput( input.inputName, value );
				}
			}
		}
		public void WireConnect( WireInputEntity inputEnt, string outputName, string inputName )
		{
			GetOutput( outputName ).connected.Add( inputEnt.GetInput( inputName ) );
		}

		public WireOutput GetOutput( string inputName )
		{
			if ( !allOutputs.ContainsKey( this.NetworkIdent ) ) {
				InitializeOutputs();
			}
			return allOutputs[this.NetworkIdent][inputName];
		}
		public string[] GetOutputNames()
		{
			if ( !allOutputs.ContainsKey( this.NetworkIdent ) ) {
				InitializeOutputs();
			}
			return allOutputs[this.NetworkIdent].Keys.ToArray();
		}

		protected void InitializeOutputs()
		{
			allOutputs[this.NetworkIdent] = new Dictionary<string, WireOutput>();
			foreach ( var outputName in WireGetOutputs() ) {
				allOutputs[this.NetworkIdent][outputName] = new WireOutput( (Entity)this, outputName );
			}
		}
		abstract public string[] WireGetOutputs();
	}
}
