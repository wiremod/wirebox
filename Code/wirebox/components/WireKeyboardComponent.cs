using Sandbox.Movement;
namespace Wirebox.Components;

[Library( "ent_wirekeyboard", Title = "Wire Keyboard" )]
public partial class WireKeyboardComponent : BaseWireOutputComponent, Component.IPressable
{
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

	private Player ActivePlayer;

	bool IPressable.CanPress( IPressable.Event e )
	{
		return true;
	}
	bool IPressable.Press( IPressable.Event e )
	{
		if ( ActivePlayer == null )
		{
			var pressedConnection = e.Source.Network.Owner;
			if ( Player.FindPlayerByConnection( pressedConnection ) is not Player player )
				return false;
			EnableKeyboard( player );
		}
		else
		{
			DisableKeyboard();
		}
		return true;
	}

	private void EnableKeyboard( Player player )
	{
		ActivePlayer = player;
		player.Tags.Add( "lockedposition" );
		player.Controller.GetComponent<LockedPositionMode>().OnInput += OnSimulatePlayer;
		// player.EnableSolidCollisions = false;
		this.WireTriggerOutput( "Active", true );
		player.Inventory.SwitchWeapon( "weapon_fists" );
	}

	private void DisableKeyboard()
	{
		var player = ActivePlayer;
		if ( player.IsValid() )
		{
			player.Tags.Remove( "lockedposition" );
			player.Controller.GetComponent<LockedPositionMode>().OnInput -= OnSimulatePlayer;
			// player.EnableSolidCollisions = true;
		}
		this.WireTriggerOutput( "Active", false );
		ActivePlayer = null;
	}

	protected override void OnDisabled()
	{
		DisableKeyboard();
	}

	private void OnSimulatePlayer( Rotation eyes, Vector3 input )
	{
		if ( Input.EscapePressed || !MainMenuPanel.Current.IsHidden )
		{
			Input.EscapePressed = false;
			DisableKeyboard();
			return;
		}
		foreach ( var name in inputButtons.Keys )
		{
			var inputButton = inputButtons[name];
			var newState = Input.Down( inputButton );
			if ( newState != (bool)this.GetOutput( name ).value )
			{
				this.WireTriggerOutput( name, newState );
			}
			this.WireTriggerOutput( "Eyes", eyes.Forward );
			this.WireTriggerOutput( "Move", input.WithZ( (Input.Down( "Duck" ) ? 1 : 0) - (Input.Down( "Jump" ) ? 1 : 0) ) );
			this.WireTriggerOutput( "MouseWheel", Input.MouseWheel.y );
		}
	}

	public override PortType[] WireGetOutputs()
	{
		return new PortType[] {
				PortType.Bool( "Active" ),
				PortType.Vector3( "Move" ),
				PortType.Vector3( "Eyes" ),
				PortType.Float( "MouseWheel" ),
			}
			.Concat( inputButtons.Keys.Select( x => PortType.Bool( x ) ) )
			.ToArray();
	}
}
