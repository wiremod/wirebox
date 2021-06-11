using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

[Library( "ent_wiregate", Title = "Wire Gate" )]
public partial class WireGateEntity : Prop, WireInputEntity, WireOutputEntity
{
	public string GateType = "Add";
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();
	public void WireInitialize()
	{
		var inputs = ((IWireEntity)this).WirePorts.inputs;
		if ( GateType == "Add" ) {
			BulkRegisterInputHandlers( ( float value ) => {
				this.WireTriggerOutput( "Out",
					  Convert.ToSingle( inputs["A"].value )
					+ Convert.ToSingle( inputs["B"].value )
					+ Convert.ToSingle( inputs["C"].value )
					+ Convert.ToSingle( inputs["D"].value )
					+ Convert.ToSingle( inputs["E"].value )
					+ Convert.ToSingle( inputs["F"].value )
					+ Convert.ToSingle( inputs["G"].value )
					+ Convert.ToSingle( inputs["H"].value )
				);
			}, new string[] { "A", "B", "C", "D", "E", "F", "G", "H" } );
		}
	}

	protected void BulkRegisterInputHandlers( Action<float> handler, string[] inputNames )
	{
		foreach ( var inputName in inputNames ) {
			this.RegisterInputHandler( inputName, handler );
		}
	}
	public string[] WireGetOutputs()
	{
		return new string[] { "Out" };
	}
}
