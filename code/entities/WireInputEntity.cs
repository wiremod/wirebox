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

		public Entity entity;
		public string inputName;
		public string type;
		public WireOutput connectedOutput;
		public WireCable AttachRope { get; set; }

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
			if ( !WirePorts.inputsInitialized ) { // these get cleared by hot reloading
				WireInitialize();
				WirePorts.inputsInitialized = true;
			}
			WirePorts.inputs[inputName].value = value;
			WirePorts.inputHandlers[inputName]( value );
		}
		public virtual void WireInitialize() { }

		public WireInput GetInput( string inputName )
		{
			if ( !WirePorts.inputsInitialized ) {
				WireInitialize();
				WirePorts.inputsInitialized = true;
			}
			return WirePorts.inputs[inputName];
		}
		public string[] GetInputNames( bool withValues = false )
		{
			if ( !WirePorts.inputsInitialized ) {
				WireInitialize();
				WirePorts.inputsInitialized = true;
			}
			return !withValues
				? WirePorts.inputs.Keys.ToArray()
				: WirePorts.inputs.Keys.Select( ( string key ) => {
					return $"{key} [{WirePorts.inputs[key].type}]: {WirePorts.inputs[key].value}";
				} ).ToArray();
		}

		public void DisconnectInput( string inputName )
		{
			DisconnectInput( GetInput( inputName ) );
		}
		public void DisconnectInput( WireInput input )
		{
			if ( input.connectedOutput != null ) {
				input.connectedOutput.connected.Remove( input );
				input.connectedOutput = null;
			}
			if ( input.AttachRope != null ) {
				input.AttachRope.Destroy( true );
				input.AttachRope = null;
			}
		}

	}

	// Extension methods to allow calling the interface methods without explicit casting
	public static class WireInputEntityUtils
	{
		public static void RegisterInputHandler( this WireInputEntity instance, string inputName, Action<float> handler )
		{
			instance.WirePorts.inputHandlers[inputName] = (( value ) => {
				if ( value is bool valueBool ) {
					handler( valueBool ? 1.0f : 0.0f );
				}
				else {
					handler( Convert.ToSingle( value ) );
				}
			});
			instance.WirePorts.inputs[inputName] = new WireInput( (Entity)instance, inputName, "float" );
		}
		public static void RegisterInputHandler( this WireInputEntity instance, string inputName, Action<bool> handler )
		{
			instance.WirePorts.inputHandlers[inputName] = (( value ) => {
				if ( value is int valueInt ) {
					handler( valueInt != 0 );
				}
				else if ( value is float valueFloat ) {
					handler( valueFloat != 0.0f );
				}
				else {
					handler( (bool)value );
				}
			});
			instance.WirePorts.inputs[inputName] = new WireInput( (Entity)instance, inputName, "bool" );
		}
		public static void RegisterInputHandler( this WireInputEntity instance, string inputName, Action<int> handler )
		{
			instance.WirePorts.inputHandlers[inputName] = (( value ) => {
				handler( (int)value );
			});
			instance.WirePorts.inputs[inputName] = new WireInput( (Entity)instance, inputName, "int" );
		}
	}

}
