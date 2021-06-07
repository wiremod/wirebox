using System.Collections.Generic;
using System.Linq;

namespace Sandbox
{
	public class WireInput
	{
		public int value;
		public Entity entity;
		public string inputName;
		public List<WireOutput> connected = new List<WireOutput>();

		public WireInput( Entity entity, string inputName )
		{
			this.entity = entity;
			this.inputName = inputName;
		}
	}
	public interface WireInputEntity
	{
		protected static Dictionary<int, Dictionary<string, WireInput>> allInputs = new Dictionary<int, Dictionary<string, WireInput>>();

		public int NetworkIdent { get; }
		public void WireTriggerInput<T>( string inputName, T value )
		{
			HandleWireInput( inputName, value );
		}
		public virtual void HandleWireInput<T>( string inputName, T value ) { }

		public WireInput GetInput( string inputName )
		{
			if ( !allInputs.ContainsKey( this.NetworkIdent ) ) {
				InitializeInputs();
			}
			return allInputs[this.NetworkIdent][inputName];
		}
		public string[] GetInputNames()
		{
			if ( !allInputs.ContainsKey( this.NetworkIdent ) ) {
				InitializeInputs();
			}
			return allInputs[this.NetworkIdent].Keys.ToArray();
		}

		protected void InitializeInputs()
		{
			allInputs[this.NetworkIdent] = new Dictionary<string, WireInput>();
			foreach ( var inputName in WireGetInputs() ) {
				allInputs[this.NetworkIdent][inputName] = new WireInput( (Entity)this, inputName );
			}
		}
		abstract public string[] WireGetInputs();
	}

}
