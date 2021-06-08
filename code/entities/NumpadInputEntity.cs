using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

[Library( "ent_numpadinput", Title = "Numpad Input" )]
public partial class NumpadInputEntity : Prop, WireOutputEntity
{
	public Entity WireOwner;
	public bool IsToggle { get; set; } = false;
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();

	private static Dictionary<string, InputButton> inputButtons = new Dictionary<string, InputButton> {
		["Forward"] = InputButton.Forward, //  W
		["Back"] = InputButton.Back, //  S
		["Left"] = InputButton.Left, //  A
		["Right"] = InputButton.Right, //  D
		["Attack1"] = InputButton.Attack1, //  Mouse0
		["Attack2"] = InputButton.Attack2, //  Mouse1
		["Reload"] = InputButton.Reload, //  R
		["Drop"] = InputButton.Drop, //  G
		["Jump"] = InputButton.Jump, //  Spacebar
		["Run"] = InputButton.Run, //  LShift and RShift
		["Walk"] = InputButton.Walk, //  LAlt
		["Duck"] = InputButton.Duck, //  LCtrl and RCtrl
		["Score"] = InputButton.Score, // Tab
		["Menu"] = InputButton.Menu, // Q
		["Flashlight"] = InputButton.Flashlight, // F
		["View"] = InputButton.View, // C
		["Voice"] = InputButton.Voice, // V
		["Slot1"] = InputButton.Slot1,
		["Slot2"] = InputButton.Slot2,
		["Slot3"] = InputButton.Slot3,
		["Slot4"] = InputButton.Slot4,
		["Slot5"] = InputButton.Slot5,
		["Slot6"] = InputButton.Slot6,
		["Slot7"] = InputButton.Slot7,
		["Slot8"] = InputButton.Slot8,
		["Slot9"] = InputButton.Slot9,
	};

	[Event.Tick.Server]
	public void OnTick()
	{
		if ( !WireOwner.IsValid() || WireOwner is not SandboxPlayer ) {
			return;
		}
		var input = ((SandboxPlayer)WireOwner).LastInput;
		if ( input == null ) {
			return;
		}
		foreach ( var name in inputButtons.Keys ) {
			var inputButton = inputButtons[name];
			var newState = input.Down( inputButton ) ? 1 : 0;
			if ( newState != ((WireOutputEntity)this).GetOutput( name ).value ) {
				WireOutputEntity.WireTriggerOutput( this, name, newState );
			}
		}
	}
	public string[] WireGetOutputs()
	{
		return inputButtons.Keys.ToArray();
	}
}
