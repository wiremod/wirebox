using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

[Library( "ent_wiregate", Title = "Wire Gate" )]
public partial class WireGateEntity : Prop, WireInputEntity, WireOutputEntity, IUse, IPhysicsUpdate
{
	[Net]
	public string GateType { get; set; } = "Add";

	private int constantValue = 3;
	private float storedFloat = 0;

	[Net]
	public string DebugText { get; set; } = "";
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();

	// todo: allow these to be extended by addons
	public static Dictionary<string, string[]> GetGates()
	{
		return new Dictionary<string, string[]> {
			["Math"] = new string[] { "Constant", "Add", "Subtract", "Multiply", "Divide", "Negate", "Absolute", "Sin", "Cos" },
			["Logic"] = new string[] { "Not", "And", "Or", "GreaterThan", "LessThan", "Equal" },
			["Comparison"] = new string[] { "Max", "Min", "Clamp" },
			["Time"] = new string[] { "Delta", "Tick", "Smoother" },
		};
	}

	public void Update( string newGateType )
	{
		var oldInputs = ((IWireEntity)this).WirePorts.inputs;
		((IWireEntity)this).WirePorts.inputs = new();

		GateType = newGateType;
		WireInitialize();


		// reconnect old matching inputs
		foreach ( var kv in ((IWireEntity)this).WirePorts.inputs ) {
			var inputName = kv.Key;
			if ( oldInputs.ContainsKey( inputName ) ) {
				var output = oldInputs[inputName].connectedOutput;
				if ( output != null && output.entity is WireOutputEntity outputEnt ) {
					var rope = oldInputs[inputName].AttachRope;
					oldInputs[inputName].AttachRope = null;
					((WireInputEntity)this).DisconnectInput( oldInputs[inputName] );
					oldInputs.Remove( inputName );

					outputEnt.WireConnect( this, output.outputName, inputName );
					((IWireEntity)this).WirePorts.inputs[inputName].AttachRope = rope;
				}
			}
		}
		foreach ( var kv in oldInputs ) {
			((WireInputEntity)this).DisconnectInput( kv.Value );
		}

		// todo: outputs, once we got more than 1
	}

	public void WireInitialize()
	{
		var inputs = ((IWireEntity)this).WirePorts.inputs;
		if ( GateType == "Add" ) {
			BulkRegisterInputHandlers( ( float value ) => {
				var outValue =
					  inputs["A"].asFloat
					+ inputs["B"].asFloat
					+ inputs["C"].asFloat
					+ inputs["D"].asFloat
					+ inputs["E"].asFloat
					+ inputs["F"].asFloat
					+ inputs["G"].asFloat
					+ inputs["H"].asFloat;
				this.WireTriggerOutput( "Out", outValue );
			}, new string[] { "A", "B", "C", "D", "E", "F", "G", "H" } );
		}
		else if ( GateType == "Subtract" ) {
			BulkRegisterInputHandlers( ( float value ) => {
				var outValue =
					  inputs["A"].asFloat
					- inputs["B"].asFloat
					- inputs["C"].asFloat
					- inputs["D"].asFloat
					- inputs["E"].asFloat
					- inputs["F"].asFloat
					- inputs["G"].asFloat
					- inputs["H"].asFloat;
				this.WireTriggerOutput( "Out", outValue );
			}, new string[] { "A", "B", "C", "D", "E", "F", "G", "H" } );
		}
		else if ( GateType == "Multiply" ) {
			BulkRegisterInputHandlers( ( float value ) => {
				var connectedInputs = inputs.Values.Where( ( input ) => input.connectedOutput != null );
				float outValue = 1;
				foreach ( var input in connectedInputs ) {
					outValue *= input.asFloat;
				}
				this.WireTriggerOutput( "Out", outValue );
			}, new string[] { "A", "B", "C", "D", "E", "F", "G", "H" } );
		}
		else if ( GateType == "Divide" ) {
			BulkRegisterInputHandlers( ( float value ) => {
				var b = inputs["B"].asFloat;
				if ( b == 0 ) {
					this.WireTriggerOutput( "Out", 0 );
				}
				else {
					this.WireTriggerOutput( "Out", inputs["A"].asFloat / b );
				}
			}, new string[] { "A", "B" } );
		}
		else if ( GateType == "Negate" ) {
			this.RegisterInputHandler( "A", ( float value ) => {
				this.WireTriggerOutput( "Out", -value );
			} );
		}
		else if ( GateType == "Sin" ) {
			this.RegisterInputHandler( "A", ( float value ) => {
				this.WireTriggerOutput( "Out", Math.Sin( value ) );
			} );
		}
		else if ( GateType == "Cos" ) {
			this.RegisterInputHandler( "A", ( float value ) => {
				this.WireTriggerOutput( "Out", Math.Cos( value ) );
			} );
		}
		else if ( GateType == "Absolute" ) {
			this.RegisterInputHandler( "A", ( float value ) => {
				this.WireTriggerOutput( "Out", Math.Abs( value ) );
			} );
		}
		else if ( GateType == "Negate" ) {
			this.RegisterInputHandler( "A", ( float value ) => {
				this.WireTriggerOutput( "Out", -value );
			} );
		}
		else if ( GateType == "Delta" ) {
			this.RegisterInputHandler( "A", ( float value ) => {
				this.WireTriggerOutput( "Out", value - storedFloat );
				storedFloat = value;
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
						 || input.asBool
				);
				this.WireTriggerOutput( "Out", outValue );
			}, new string[] { "A", "B", "C", "D", "E", "F", "G", "H" } );
		}
		else if ( GateType == "Or" ) {
			BulkRegisterInputHandlers( ( bool value ) => {
				var outValue = inputs.Values.Any( input =>
					 input.asBool
				);
				this.WireTriggerOutput( "Out", outValue );
			}, new string[] { "A", "B", "C", "D", "E", "F", "G", "H" } );
		}
		else if ( GateType == "GreaterThan" ) {
			BulkRegisterInputHandlers( ( float value ) => {
				var a = inputs["A"].asFloat;
				var b = inputs["B"].asFloat;
				this.WireTriggerOutput( "Out", a > b );
			}, new string[] { "A", "B" } );
		}
		else if ( GateType == "LessThan" ) {
			BulkRegisterInputHandlers( ( float value ) => {
				var a = inputs["A"].asFloat;
				var b = inputs["B"].asFloat;
				this.WireTriggerOutput( "Out", a < b );
			}, new string[] { "A", "B" } );
		}
		else if ( GateType == "Equal" ) {
			BulkRegisterInputHandlers( ( float value ) => {
				var a = inputs["A"].asFloat;
				var b = inputs["B"].asFloat;
				this.WireTriggerOutput( "Out", a == b );
			}, new string[] { "A", "B" } );
		}
		else if ( GateType == "Constant" ) {
			DebugText = $" value: {constantValue}";
			this.WireTriggerOutput( "Out", constantValue );
		}
		else if ( GateType == "Min" ) {
			BulkRegisterInputHandlers( ( float value ) => {
				var connectedInputs = inputs.Values.Where( ( input ) => input.connectedOutput != null );
				float outValue = connectedInputs.FirstOrDefault()?.asFloat ?? 0f;
				foreach ( var x in connectedInputs ) {
					outValue = Math.Min( outValue, x.asFloat );
				}
				this.WireTriggerOutput( "Out", outValue );
			}, new string[] { "A", "B", "C", "D", "E", "F", "G", "H" } );
		}
		else if ( GateType == "Max" ) {
			BulkRegisterInputHandlers( ( float value ) => {
				var connectedInputs = inputs.Values.Where( ( input ) => input.connectedOutput != null );
				float outValue = 0;
				foreach ( var x in connectedInputs ) {
					outValue = Math.Max( outValue, x.asFloat );
				}
				this.WireTriggerOutput( "Out", outValue );
			}, new string[] { "A", "B", "C", "D", "E", "F", "G", "H" } );
		}
		else if ( GateType == "Clamp" ) {
			BulkRegisterInputHandlers( ( float value ) => {
				this.WireTriggerOutput( "Out", Math.Clamp(
					inputs["Value"].asFloat,
					inputs["Min"].asFloat,
					inputs["Max"].asFloat
				) );
			}, new string[] { "Min", "Max", "Value" } );
		}
		else if ( GateType == "Smoother" ) {
			this.RegisterInputHandler( "A", ( float value ) => {} );
			this.RegisterInputHandler( "Rate", ( float value ) => {} );
			inputs["Rate"].value = 1.0f; // a default
			this.RegisterInputHandler( "Clk", ( bool value ) => { // todo: event type
				var A = inputs["A"].asFloat;
				if (A > storedFloat) {
					storedFloat = Math.Min(A, storedFloat + inputs["Rate"].asFloat);
					this.WireTriggerOutput( "Out", storedFloat );
				} else if (A < storedFloat) {
					storedFloat = Math.Max(A, storedFloat - inputs["Rate"].asFloat);
					this.WireTriggerOutput( "Out", storedFloat );
				}
			} );
		}
	}

	public void OnPostPhysicsStep( float dt )
	{
		// todo: it sucks to bind this for gates that don't need it, perhaps move to separate entity?
		if ( GateType == "Tick" ) {
			this.WireTriggerOutput( "Out", Time.Tick );
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
