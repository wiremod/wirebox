using System.Collections.Generic;
using System.Linq;

namespace Sandbox
{
	public class WireOutput
	{
		public object value = 0;
		public Entity entity;
		public string outputName;
		public string type;
		public List<WireInput> connected = new();
		public int executions = 0;
		public int executionsTick = 0;

		public WireOutput( Entity entity, string outputName, string type )
		{
			this.entity = entity;
			this.outputName = outputName;
			this.type = type;

			value = IWireEntity.GetDefaultValueFromType( type );
		}
	}

	public readonly struct PortType
	{
		public string Name { get; init; }
		public string Type { get; init; }

		public static PortType Any( string name ) =>
			new() { Name = name, Type = "any" };
		public static PortType Bool( string name ) =>
			new() { Name = name, Type = "bool" };
		public static PortType Int( string name ) =>
			new() { Name = name, Type = "int" };
		public static PortType Float( string name ) =>
			new() { Name = name, Type = "float" };
		public static PortType String( string name ) =>
			new() { Name = name, Type = "string" };
		public static PortType Vector3( string name ) =>
			new() { Name = name, Type = "vector3" };
		public static PortType Angle( string name ) =>
			new() { Name = name, Type = "angle" };
		public static PortType Rotation( string name ) =>
			new() { Name = name, Type = "rotation" };
		public static PortType Entity( string name ) =>
			new() { Name = name, Type = "entity" };
	}

	public interface IWireOutputEntity : IWireEntity
	{
		public void WireTriggerOutput<T>( string outputName, T value )
		{
			var output = GetOutput( outputName );

			// return early if new value is the same as current value, so nothing should trigger
			if ( output.value.Equals( value ) )
				return;

			output.value = value;

			if ( output.executionsTick != Time.Tick )
			{
				output.executionsTick = Time.Tick;
				output.executions = 1;
			}
			else if ( output.executions >= 4 )
			{
				// prevent infinite loops
				return; // todo: queue for next tick?
			}
			else
			{
				output.executions++;
			}

			foreach ( var input in output.connected )
			{
				if ( !input.entity.IsValid() ) continue;
				if ( input.entity is IWireInputEntity inputEntity )
				{
					inputEntity.WireTriggerInput( input.inputName, value );
				}
			}
		}
		public void WireConnect( IWireInputEntity inputEnt, string outputName, string inputName )
		{
			var input = inputEnt.GetInput( inputName );
			var output = GetOutput( outputName );
			var connected = output.connected;
			if ( input.connectedOutput != null )
			{
				inputEnt.DisconnectInput( inputName );
			}
			input.connectedOutput = output;
			connected.Add( input );
			inputEnt.WireTriggerInput( input.inputName, output.value );
		}

		public WireOutput GetOutput( string inputName )
		{
			if ( WirePorts.outputs.Count == 0 )
			{
				InitializeOutputs();
				WireInitializeOutputs();
			}
			return WirePorts.outputs[inputName];
		}
		public string[] GetOutputNames( bool withValues = false )
		{
			if ( WirePorts.outputs.Count == 0 )
			{
				InitializeOutputs();
				WireInitializeOutputs();
			}
			return !withValues
				? WirePorts.outputs.Keys.ToArray()
				: WirePorts.outputs.Keys.Select( ( string key ) =>
				{
					var type = WirePorts.outputs[key].type;
					if ( type == "string" )
						return $"{key} [{type}]: \"{WirePorts.outputs[key].value}\"";

					return $"{key} [{type}]: {WirePorts.outputs[key].value}";
				} ).ToArray();
		}

		// Entities can implement this for custom output initialization
		public virtual void WireInitializeOutputs() { }

		public void InitializeOutputs()
		{
			foreach ( var type in WireGetOutputs() )
			{
				WirePorts.outputs[type.Name] = new WireOutput( (Entity)this, type.Name, type.Type );
			}
		}
		abstract public PortType[] WireGetOutputs();
	}

	// Extension methods to allow calling the interface methods without explicit casting
	public static class IWireOutputEntityUtils
	{
		public static void WireTriggerOutput<T>( this IWireOutputEntity instance, string outputName, T value )
		{
			instance.WireTriggerOutput( outputName, value );
		}
	}

}
