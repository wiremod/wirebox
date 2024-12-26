using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

[Library( "ent_wirekeyboard", Title = "Wire Keyboard" )]
public partial class KeyboardEntity : Prop, IUse, IWireOutputEntity
{
	public bool IsToggle { get; set; } = false;
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();

	private static readonly Dictionary<string, string> inputButtons = new Dictionary<string, string>
	{
		["Forward"] = "Forward", //  W
		["Backward"] = "Backward", //  S
		["Left"] = "Left", //  A
		["Right"] = "Right", //  D
		["Attack1"] = "Attack1", //  Mouse0
		["Attack2"] = "Attack2", //  Mouse1
		["Reload"] = "Reload", //  R
		["Drop"] = "Drop", //  G
		["Jump"] = "Jump", //  Spacebar
		["Run"] = "Run", //  LShift and RShift
		["Walk"] = "Walk", //  LAlt
		["Duck"] = "Duck", //  LCtrl and RCtrl
		["Score"] = "Score", // Tab
		["Menu"] = "Menu", // Q
		["Flashlight"] = "Flashlight", // F
		["View"] = "View", // C
		["Voice"] = "Voice", // V
		["Slot1"] = "Slot1",
		["Slot2"] = "Slot2",
		["Slot3"] = "Slot3",
		["Slot4"] = "Slot4",
		["Slot5"] = "Slot5",
		["Slot6"] = "Slot6",
		["Slot7"] = "Slot7",
		["Slot8"] = "Slot8",
		["Slot9"] = "Slot9",
	};

	public bool IsUsable( Entity user )
	{
		return (user as IEntity) is SandboxPlayer;
	}
	public bool OnUse( Entity user )
	{
		if ( (user as IEntity) is SandboxPlayer player )
		{
			if ( player.Controller.GetType() != typeof( LockedPositionController ) )
			{
				player.OnSimulate -= OnSimulatePlayer;
				player.OnSimulate += OnSimulatePlayer;
				player.EnableSolidCollisions = false;
				player.Controller = new LockedPositionController();

				this.WireTriggerOutput( "Active", true );
			}
			else
			{
				player.OnSimulate -= OnSimulatePlayer;
				player.EnableSolidCollisions = true;
				player.Controller = new WalkController
				{
					WalkSpeed = 60f,
					DefaultSpeed = 180.0f
				};

				this.WireTriggerOutput( "Active", false );
			}
		}
		return false;
	}

	private void OnSimulatePlayer( SandboxPlayer player )
	{
		foreach ( var name in inputButtons.Keys )
		{
			var inputButton = inputButtons[name];
			var newState = Input.Down( inputButton );
			if ( newState != (bool)((IWireOutputEntity)this).GetOutput( name ).value )
			{
				this.WireTriggerOutput( name, newState );
			}
		}
	}
	public PortType[] WireGetOutputs()
	{
		return inputButtons.Keys
			.Concat( new string[] { "Active" } )
			.Select( x => PortType.Bool( x ) )
			.ToArray();
	}
}
