using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

[Library( "ent_wiredigitalscreen", Title = "Wire Digital Screen" )]
public partial class WireDigitalScreenEntity : Prop, IWireInputEntity
{
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();
	[Net]
	private float valueA { get; set; } = 0;

	private string labelPrefix = "Wire Screen: ";
	private DigitalScreenWorldPanel worldPanel;

	public void WireInitialize()
	{
		this.RegisterInputHandler( "A", ( float value ) =>
		{
			valueA = float.Round( value, 1 );
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
	}

	[GameEvent.Client.Frame]
	protected void UpdatePanel()
	{
		if ( !Game.IsClient || !worldPanel.IsValid ) return;
		var modelData = ScreenTransforms.GetValueOrDefault( Model.Name );
		worldPanel.Position = Transform.PointToWorld( modelData.Position );
		worldPanel.Rotation = Rotation.RotateAroundAxis( modelData.Rotation.Right, -modelData.Rotation.w );
		worldPanel.Label.Text = labelPrefix + "\n" + valueA;
	}

	[Event( "spawnlists.initialize" )]
	public static void SpawnlistsInitialize()
	{
		ModelSelector.AddToSpawnlist( "screen", new string[] {
			Cloud.Asset("https://asset.party/baik/flatscreen_tv"),
			Cloud.Asset("https://asset.party/eurorp/monitor"),
		} );
	}

	public static readonly Dictionary<string, Transform> ScreenTransforms = new()
	{
		["models/television/flatscreen_tv.vmdl"] = new Transform( new Vector3( 13, 3, 1.75f ), new Rotation( new Vector3( 0, 1, 0 ), -90 ) ),
		["models/others/monitor/monitor.vmdl"] = new Transform( new Vector3( -0.5f, 9, 1.25f ), Rotation.Identity ),
	};
}


class DigitalScreenWorldPanel : WorldPanel
{
	public Label Label;
	public DigitalScreenWorldPanel()
	{
		Label = Add.Label( "Wire Screen: \n0", "ds-text" );
	}
}
