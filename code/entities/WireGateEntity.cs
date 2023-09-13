using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

[Library( "ent_wiregate", Title = "Wire Gate" )]
public partial class WireGateEntity : Prop, IWireInputEntity, IWireOutputEntity, IUse
{
	[Net]
	public string GateType { get; set; } = "Add";

	private int constantValue = 3;
	private float storedFloat = 0;
	private Entity storedEnt;
	private float[] storedRAM;

	[Net]
	public string DebugText { get; set; } = "";
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();

	// todo: allow these to be extended by addons
	public static Dictionary<string, string[]> GetGates()
	{
		return new Dictionary<string, string[]>
		{
			["Math"] = new string[] { "Constant", "Add", "Subtract", "Multiply", "Divide", "Mod", "Negate", "Absolute", "Sin", "Cos" },
			["Logic"] = new string[] { "Not", "And", "Or", "GreaterThan", "LessThan", "Equal" },
			["Comparison"] = new string[] { "Max", "Min", "Clamp" },
			["Time"] = new string[] { "Delta", "Tick", "Smoother" },
			["Entity"] = new string[] { "Position", "Velocity", "Owner" },
			["Memory"] = new string[] { "Latch", "D-Latch", "Toggle", "RAM", "Incrementor" },
		};
	}

	public void Update( string newGateType )
	{
		var oldInputs = ((IWireEntity)this).WirePorts.inputs;
		((IWireEntity)this).WirePorts.inputs = new();

		GateType = newGateType;
		WireInitialize();


		// reconnect old matching inputs
		foreach ( var kv in ((IWireEntity)this).WirePorts.inputs )
		{
			var inputName = kv.Key;
			if ( oldInputs.ContainsKey( inputName ) )
			{
				var output = oldInputs[inputName].connectedOutput;
				if ( output != null && output.entity is IWireOutputEntity outputEnt )
				{
					var rope = oldInputs[inputName].AttachRope;
					oldInputs[inputName].AttachRope = null;
					((IWireInputEntity)this).DisconnectInput( oldInputs[inputName] );
					oldInputs.Remove( inputName );

					outputEnt.WireConnect( this, output.outputName, inputName );
					((IWireEntity)this).WirePorts.inputs[inputName].AttachRope = rope;
				}
			}
		}
		foreach ( var kv in oldInputs )
		{
			((IWireInputEntity)this).DisconnectInput( kv.Value );
		}

		// todo: outputs, once we got more than 1
	}

	public void WireInitialize()
	{
		if ( GetModelName().Contains( "chip_rectangle.vmdl" ) )
		{
			var allGates = GetGates();
			if ( GateType == "Constant" )
			{
				SetMaterialGroup( 6 );
			}
			else if ( allGates["Math"].Contains( GateType ) )
			{
				SetMaterialGroup( 2 );
			}
			else if ( allGates["Logic"].Contains( GateType ) )
			{
				SetMaterialGroup( 3 );
			}
			else if ( allGates["Comparison"].Contains( GateType ) )
			{
				SetMaterialGroup( 4 );
			}
			else if ( allGates["Time"].Contains( GateType ) )
			{
				SetMaterialGroup( 5 );
			}
			else
			{
				SetMaterialGroup( 0 );
			}
		}

		var inputs = ((IWireEntity)this).WirePorts.inputs;
		if ( GateType == "Add" )
		{
			Action<object> handler = ( object value ) =>
			{
				if ( inputs["A"].value is Vector3 )
				{
					var outValue =
						  inputs["A"].asVector3
						+ inputs["B"].asVector3
						+ inputs["C"].asVector3
						+ inputs["D"].asVector3
						+ inputs["E"].asVector3
						+ inputs["F"].asVector3
						+ inputs["G"].asVector3
						+ inputs["H"].asVector3;
					this.WireTriggerOutput( "Out", outValue );
				}
				else
				{
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
				}
			};
			BulkRegisterInputHandlers( handler, new string[] { "A", "B", "C", "D", "E", "F", "G", "H" } );
		}
		else if ( GateType == "Subtract" )
		{
			Action<object> handler = ( object value ) =>
			{
				if ( inputs["A"].value is Vector3 )
				{
					var outValue =
						  inputs["A"].asVector3
						- inputs["B"].asVector3
						- inputs["C"].asVector3
						- inputs["D"].asVector3
						- inputs["E"].asVector3
						- inputs["F"].asVector3
						- inputs["G"].asVector3
						- inputs["H"].asVector3;
					this.WireTriggerOutput( "Out", outValue );
				}
				else
				{
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
				}
			};
			BulkRegisterInputHandlers( handler, new string[] { "A", "B", "C", "D", "E", "F", "G", "H" } );
		}
		else if ( GateType == "Multiply" )
		{
			BulkRegisterInputHandlers( ( float value ) =>
			{
				var connectedInputs = inputs.Values.Where( ( input ) => input.connectedOutput != null );
				float outValue = 1;
				foreach ( var input in connectedInputs )
				{
					outValue *= input.asFloat;
				}
				this.WireTriggerOutput( "Out", outValue );
			}, new string[] { "A", "B", "C", "D", "E", "F", "G", "H" } );
		}
		else if ( GateType == "Divide" )
		{
			BulkRegisterInputHandlers( ( float value ) =>
			{
				var b = inputs["B"].asFloat;
				if ( b == 0 )
				{
					this.WireTriggerOutput( "Out", 0 );
				}
				else
				{
					this.WireTriggerOutput( "Out", inputs["A"].asFloat / b );
				}
			}, new string[] { "A", "B" } );
		}
		else if ( GateType == "Mod" )
		{
			BulkRegisterInputHandlers( ( float value ) =>
			{
				var b = inputs["B"].asFloat;
				if ( b == 0 )
				{
					this.WireTriggerOutput( "Out", 0 );
				}
				else
				{
					this.WireTriggerOutput( "Out", inputs["A"].asFloat % b );
				}
			}, new string[] { "A", "B" } );
		}
		else if ( GateType == "Sin" )
		{
			this.RegisterInputHandler( "A", ( float value ) =>
			{
				this.WireTriggerOutput( "Out", Math.Sin( value ) );
			} );
		}
		else if ( GateType == "Cos" )
		{
			this.RegisterInputHandler( "A", ( float value ) =>
			{
				this.WireTriggerOutput( "Out", Math.Cos( value ) );
			} );
		}
		else if ( GateType == "Absolute" )
		{
			this.RegisterInputHandler( "A", ( float value ) =>
			{
				this.WireTriggerOutput( "Out", Math.Abs( value ) );
			} );
		}
		else if ( GateType == "Negate" )
		{
			this.RegisterInputHandler( "A", ( float value ) =>
			{
				this.WireTriggerOutput( "Out", -value );
			} );
		}
		else if ( GateType == "Delta" )
		{
			this.RegisterInputHandler( "A", ( float value ) =>
			{
				this.WireTriggerOutput( "Out", value - storedFloat );
				storedFloat = value;
			} );
		}
		else if ( GateType == "Not" )
		{
			this.RegisterInputHandler( "A", ( bool value ) =>
			{
				this.WireTriggerOutput( "Out", !value );
			} );
		}
		else if ( GateType == "And" )
		{
			BulkRegisterInputHandlers( ( bool value ) =>
			{
				var outValue = inputs.Values.All( input =>
					 input.connectedOutput == null
						 || input.asBool
				);
				this.WireTriggerOutput( "Out", outValue );
			}, new string[] { "A", "B", "C", "D", "E", "F", "G", "H" } );
		}
		else if ( GateType == "Or" )
		{
			BulkRegisterInputHandlers( ( bool value ) =>
			{
				var outValue = inputs.Values.Any( input =>
					 input.asBool
				);
				this.WireTriggerOutput( "Out", outValue );
			}, new string[] { "A", "B", "C", "D", "E", "F", "G", "H" } );
		}
		else if ( GateType == "GreaterThan" )
		{
			BulkRegisterInputHandlers( ( float value ) =>
			{
				var a = inputs["A"].asFloat;
				var b = inputs["B"].asFloat;
				this.WireTriggerOutput( "Out", a > b );
			}, new string[] { "A", "B" } );
		}
		else if ( GateType == "LessThan" )
		{
			BulkRegisterInputHandlers( ( float value ) =>
			{
				var a = inputs["A"].asFloat;
				var b = inputs["B"].asFloat;
				this.WireTriggerOutput( "Out", a < b );
			}, new string[] { "A", "B" } );
		}
		else if ( GateType == "Equal" )
		{
			BulkRegisterInputHandlers( ( float value ) =>
			{
				var a = inputs["A"].asFloat;
				var b = inputs["B"].asFloat;
				this.WireTriggerOutput( "Out", a == b );
			}, new string[] { "A", "B" } );
		}
		else if ( GateType == "Constant" )
		{
			DebugText = $" value: {constantValue}";
			this.WireTriggerOutput( "Out", constantValue );
		}
		else if ( GateType == "Min" )
		{
			BulkRegisterInputHandlers( ( float value ) =>
			{
				var connectedInputs = inputs.Values.Where( ( input ) => input.connectedOutput != null );
				float outValue = connectedInputs.FirstOrDefault()?.asFloat ?? 0f;
				foreach ( var x in connectedInputs )
				{
					outValue = Math.Min( outValue, x.asFloat );
				}
				this.WireTriggerOutput( "Out", outValue );
			}, new string[] { "A", "B", "C", "D", "E", "F", "G", "H" } );
		}
		else if ( GateType == "Max" )
		{
			BulkRegisterInputHandlers( ( float value ) =>
			{
				var connectedInputs = inputs.Values.Where( ( input ) => input.connectedOutput != null );
				float outValue = 0;
				foreach ( var x in connectedInputs )
				{
					outValue = Math.Max( outValue, x.asFloat );
				}
				this.WireTriggerOutput( "Out", outValue );
			}, new string[] { "A", "B", "C", "D", "E", "F", "G", "H" } );
		}
		else if ( GateType == "Clamp" )
		{
			BulkRegisterInputHandlers( ( float value ) =>
			{
				this.WireTriggerOutput( "Out", Math.Clamp(
					inputs["Value"].asFloat,
					inputs["Min"].asFloat,
					inputs["Max"].asFloat
				) );
			}, new string[] { "Min", "Max", "Value" } );
		}
		else if ( GateType == "Smoother" )
		{
			this.RegisterInputHandler( "A", ( float value ) => { } );
			this.RegisterInputHandler( "Rate", ( float value ) => { } );
			inputs["Rate"].value = 1.0f; // a default
			this.RegisterInputHandler( "Clk", ( bool value ) =>
			{ // todo: event type
				var A = inputs["A"].asFloat;
				if ( A > storedFloat )
				{
					storedFloat = Math.Min( A, storedFloat + inputs["Rate"].asFloat );
					this.WireTriggerOutput( "Out", storedFloat );
				}
				else if ( A < storedFloat )
				{
					storedFloat = Math.Max( A, storedFloat - inputs["Rate"].asFloat );
					this.WireTriggerOutput( "Out", storedFloat );
				}
			} );
		}
		else if ( GateType == "Owner" )
		{
			this.WireTriggerOutput( "Out", (this as Entity).GetPlayerOwner() );
		}
		else if ( GateType == "Position" )
		{
			this.RegisterInputHandler( "Ent", ( Entity value ) =>
			{
				storedEnt = value;
			} );
		}
		else if ( GateType == "Velocity" )
		{
			this.RegisterInputHandler( "Ent", ( Entity value ) =>
			{
				storedEnt = value;
			} );
		}
		else if ( GateType == "Latch" )
		{
			this.RegisterInputHandler( "Value", ( object value ) => { } );
			this.RegisterInputHandler( "Write", ( bool value ) =>
			{
				if ( value )
				{
					this.WireTriggerOutput( "Out", inputs["Value"].value );
				}
			} );
		}
		else if ( GateType == "D-Latch" )
		{
			Action<object> handler = ( object value ) =>
			{
				if ( inputs["On"].asBool )
				{
					this.WireTriggerOutput( "Out", inputs["Value"].value );
				}
			};
			this.RegisterInputHandler( "Value", handler );
			this.RegisterInputHandler( "On", handler );
		}
		else if ( GateType == "Toggle" )
		{
			this.RegisterInputHandler( "Toggle", ( bool value ) =>
			{
				if ( value )
				{
					float newValue = Math.Abs( storedFloat - 1 ) < 0.01 ? 0 : 1;
					storedFloat = newValue;
					this.WireTriggerOutput( "Out", storedFloat > 0.01f );
				}
			} );
		}
		else if ( GateType == "RAM" )
		{
			storedRAM ??= new float[32768];
			this.RegisterInputHandler( "Address", ( float value ) =>
			{
				if ( (int)value >= 0 && (int)value < 32768 )
				{
					this.WireTriggerOutput( "Out", storedRAM[(int)value] );
				}
			} );
			this.RegisterInputHandler( "Value", ( float value ) => { } );
			this.RegisterInputHandler( "Write", ( bool value ) =>
			{
				var address = (int)inputs["Address"].asFloat;
				if ( value && address is >= 0 and < 32768 )
				{
					storedRAM[address] = inputs["Value"].asFloat;
					this.WireTriggerOutput( "Out", inputs["Value"].asFloat );
				}
			} );
			this.RegisterInputHandler( "Reset", ( bool value ) =>
			{
				storedRAM = new float[32768];
				this.WireTriggerOutput( "Out", 0f );
			} );
		}
		else if ( GateType == "Incrementor" )
		{
			this.RegisterInputHandler( "Increment", ( bool value ) =>
			{
				if ( value )
				{
					storedFloat++;
					this.WireTriggerOutput( "Out", storedFloat );
				}
			} );
			this.RegisterInputHandler( "Decrement", ( bool value ) =>
			{
				if ( value )
				{
					storedFloat--;
					this.WireTriggerOutput( "Out", storedFloat );
				}
			} );
			this.RegisterInputHandler( "Reset", ( bool value ) =>
			{
				if ( value )
				{
					storedFloat = 0;
					this.WireTriggerOutput( "Out", storedFloat );
				}
			} );
		}
	}

	[Event.Physics.PostStep]
	public void OnPostPhysicsStep()
	{
		// todo: it sucks to bind this for gates that don't need it, perhaps move to separate entity?
		if ( GateType == "Tick" )
		{
			this.WireTriggerOutput( "Out", Time.Tick );
		}
		else if ( GateType == "Position" )
		{
			this.WireTriggerOutput( "Out", storedEnt.IsValid() ? storedEnt.Position : Vector3.Zero );
		}
		else if ( GateType == "Velocity" )
		{
			this.WireTriggerOutput( "Out", storedEnt.IsValid() ? storedEnt.Velocity : Vector3.Zero );
		}
	}

	protected void BulkRegisterInputHandlers( Action<float> handler, string[] inputNames )
	{
		foreach ( var inputName in inputNames )
		{
			this.RegisterInputHandler( inputName, handler );
		}
	}
	protected void BulkRegisterInputHandlers( Action<bool> handler, string[] inputNames )
	{
		foreach ( var inputName in inputNames )
		{
			this.RegisterInputHandler( inputName, handler );
		}
	}
	protected void BulkRegisterInputHandlers( Action<object> handler, string[] inputNames )
	{
		foreach ( var inputName in inputNames )
		{
			this.RegisterInputHandler( inputName, handler );
		}
	}

	public PortType[] WireGetOutputs()
	{
		if ( GateType == "Owner" )
		{
			return new PortType[] { PortType.Entity( "Out" ) };
		}
		if ( GateType == "Position" || GateType == "Velocity" )
		{
			return new PortType[] { PortType.Vector3( "Out" ) };
		}
		if ( GateType is "Toggle" )
		{
			return new PortType[] { PortType.Bool( "Out" ) };
		}
		if ( GateType is "Latch" or "D-Latch" or "Add" or "Subtract" )
		{
			return new PortType[] { PortType.Any( "Out" ) };
		}
		return new PortType[] { PortType.Float( "Out" ) };
	}

	string IWireEntity.GetOverlayText()
	{
		return $"Gate: {GateType}{DebugText}";
	}

	public bool OnUse( Entity user )
	{
		if ( GateType == "Constant" && Game.IsServer )
		{
			constantValue += Input.Down( InputButton.Run ) ? -1 : 1;
			DebugText = $" value: {constantValue}";
			this.WireTriggerOutput( "Out", constantValue );
		}
		return false;
	}

	public bool IsUsable( Entity user )
	{
		return this.GateType == "Constant";
	}
}
