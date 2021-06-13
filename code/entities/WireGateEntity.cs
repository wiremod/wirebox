using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

[Library( "ent_wiregate", Title = "Wire Gate" )]
public partial class WireGateEntity : Prop, WireInputEntity, WireOutputEntity, IUse
{
	[Net]
	public string GateType { get; set; } = "Add";

	private int constantValue = 3;

	[Net]
	public string DebugText { get; set; } = "";
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();

	public static IEnumerable<string> GetGates()
	{
		return new string[] { "Constant", "Add", "Subtract", "Negate", "Not", "And", "Or" };
	}
	public void WireInitialize()
	{
		var inputs = ((IWireEntity)this).WirePorts.inputs;
		if ( GateType == "Add" ) {
			BulkRegisterInputHandlers( ( float value ) => {
				var outValue =
					  Convert.ToSingle( inputs["A"].value )
					+ Convert.ToSingle( inputs["B"].value )
					+ Convert.ToSingle( inputs["C"].value )
					+ Convert.ToSingle( inputs["D"].value )
					+ Convert.ToSingle( inputs["E"].value )
					+ Convert.ToSingle( inputs["F"].value )
					+ Convert.ToSingle( inputs["G"].value )
					+ Convert.ToSingle( inputs["H"].value );
				this.WireTriggerOutput( "Out", outValue );
			}, new string[] { "A", "B", "C", "D", "E", "F", "G", "H" } );
		}
		else if ( GateType == "Subtract" ) {
			BulkRegisterInputHandlers( ( float value ) => {
				var outValue =
					  Convert.ToSingle( inputs["A"].value )
					- Convert.ToSingle( inputs["B"].value )
					- Convert.ToSingle( inputs["C"].value )
					- Convert.ToSingle( inputs["D"].value )
					- Convert.ToSingle( inputs["E"].value )
					- Convert.ToSingle( inputs["F"].value )
					- Convert.ToSingle( inputs["G"].value )
					- Convert.ToSingle( inputs["H"].value );
				this.WireTriggerOutput( "Out", outValue );
			}, new string[] { "A", "B", "C", "D", "E", "F", "G", "H" } );
		}
		else if ( GateType == "Negate" ) {
			this.RegisterInputHandler( "A", ( float value ) => {
				this.WireTriggerOutput( "Out", -value );
			} );
		}
		else if ( GateType == "Not" ) {
			this.RegisterInputHandler( "A", ( bool value ) => {
				this.WireTriggerOutput( "Out", !value );
			} );
		}
		else if ( GateType == "And" ) {
			BulkRegisterInputHandlers( ( bool value ) => {
				var outValue = inputs.Values.All( input =>
					 input.connectedOutput == null
						 || (bool)input.value
				);
				this.WireTriggerOutput( "Out", outValue );
			}, new string[] { "A", "B", "C", "D", "E", "F", "G", "H" } );
		}
		else if ( GateType == "Or" ) {
			BulkRegisterInputHandlers( ( bool value ) => {
				var outValue = inputs.Values.Any( input =>
					 (bool)input.value
				);
				this.WireTriggerOutput( "Out", outValue );
			}, new string[] { "A", "B", "C", "D", "E", "F", "G", "H" } );
		}
		else if ( GateType == "Constant" ) {
			DebugText = $" value: {constantValue}";
			this.WireTriggerOutput( "Out", constantValue );
		}
	}

	protected void BulkRegisterInputHandlers( Action<float> handler, string[] inputNames )
	{
		foreach ( var inputName in inputNames ) {
			this.RegisterInputHandler( inputName, handler );
		}
	}
	protected void BulkRegisterInputHandlers( Action<bool> handler, string[] inputNames )
	{
		foreach ( var inputName in inputNames ) {
			this.RegisterInputHandler( inputName, handler );
		}
	}
	public string[] WireGetOutputs()
	{
		return new string[] { "Out" };
	}

	string IWireEntity.GetOverlayText()
	{
		return $"Gate: {GateType}{DebugText}";
	}

	public bool OnUse( Entity user )
	{
		if ( GateType == "Constant" && Host.IsServer ) {
			constantValue += Input.Down( InputButton.Run ) ? -1 : 1;
		}
		return false;
	}

	public bool IsUsable( Entity user )
	{
		return this.GateType == "Constant";
	}
}
