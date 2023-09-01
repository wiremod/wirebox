using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

[Library( "ent_wiredigitalscreen", Title = "Wire Digital Screen" )]
public partial class WireDigitalScreenEntity : Prop, IWireInputEntity
{
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();
	[Net]
	private string valueString { get; set; } = "0";

	private string labelPrefix = "Wire Screen: ";
	private DigitalScreenWorldPanel worldPanel;

	public void WireInitialize()
	{
		this.RegisterInputHandler( "Text", ( string value ) =>
		{
			valueString = value;
		} );
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();
		worldPanel = new DigitalScreenWorldPanel
		{
			Transform = Transform,
		};
		worldPanel.Label.Style.Width = Model.Bounds.Size.y * 8.5f;
		worldPanel.Label.Style.TextAlign = TextAlign.Center;
		worldPanel.Label.Style.FontSize = Length.Pixels( Model.Bounds.Size.y * 1.25f );
		worldPanel.Label.Style.FontColor = Color.White;
		worldPanel.Value.Style.Width = Model.Bounds.Size.y * 8.5f;
		worldPanel.Value.Style.TextAlign = TextAlign.Center;
		worldPanel.Value.Style.FontSize = Length.Pixels( Model.Bounds.Size.y );
		worldPanel.Value.Style.FontColor = Color.White;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		worldPanel?.Delete();
	}

	[GameEvent.Client.Frame]
	protected void UpdatePanel()
	{
		if ( !Game.IsClient || !worldPanel.IsValid ) return;
		var modelData = ScreenDatabase.GetValueOrDefault( Model.Name );
		worldPanel.Position = Transform.PointToWorld( modelData.Position );
		worldPanel.Rotation = Rotation.RotateAroundAxis( modelData.Rotation.Right, -modelData.Rotation.w );
		worldPanel.Label.Text = labelPrefix;
		worldPanel.Value.Text = valueString;
	}

	[Event( "spawnlists.initialize" )]
	public static void SpawnlistsInitialize()
	{
		ModelSelector.AddToSpawnlist( "screen", new string[] {
			Cloud.Asset("https://asset.party/baik/flatscreen_tv"),
			Cloud.Asset("https://asset.party/eurorp/monitor"),
		} );
		ScreenDatabase = new()
		{
			["models/television/flatscreen_tv.vmdl"] = new ScreenData()
			{
				Position = new Vector3( 13, 3, 1.75f ),
				Rotation = new Rotation( new Vector3( 0, 1, 0 ), -90 ),
				Size = new Vector2( 50, 30 ),
				ScreenTextureIndex = 1,
			},
			["models/others/monitor/monitor.vmdl"] = new ScreenData()
			{
				Position = new Vector3( -0.5f, 9, 1.25f ),
				Rotation = Rotation.Identity,
				Size = new Vector2( 37, 21 ),
				ScreenTextureIndex = 1,
			},
		};
	}

	public static Dictionary<string, ScreenData> ScreenDatabase = new();
}


class DigitalScreenWorldPanel : WorldPanel
{
	public Label Label;
	public Label Value;
	public DigitalScreenWorldPanel()
	{
		Style.FlexDirection = FlexDirection.Column;
		Label = Add.Label( "Wire Screen:", "ds-text" );
		Value = Add.Label( "0", "ds-text" );
	}
}

public struct ScreenData
{
	public Vector3 Position;
	public Rotation Rotation;
	// Size of the viewable area
	public Vector2 Size;
	// Index of the texture to override (typically 1)
	public int ScreenTextureIndex;
}
