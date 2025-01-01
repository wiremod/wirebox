using Sandbox.UI;
using Sandbox.UI.Construct;

[Library( "ent_wiredigitalscreen", Title = "Wire Digital Screen" )]
public partial class WireDigitalScreenComponent : BaseWireInputComponent
{
	[Sync]
	private string valueString { get; set; } = "0";
	[Sync]
	public string LabelPrefix { get; set; } = "Wire Screen: ";
	private Sandbox.WorldPanel worldPanelComponent;
	private Sandbox.UI.WorldPanel worldPanel;

	private Label Label;
	private Label Value;
	private Model Model;
	private GameObject mountPoint;

	public override void WireInitialize()
	{
		this.RegisterInputHandler( "Label", ( string value ) =>
		{
			LabelPrefix = value;
		} );
		this.RegisterInputHandler( "Text", ( string value ) =>
		{
			valueString = value;
		} );
	}

	protected override void OnEnabled()
	{
		base.OnEnabled();
		Model = GetComponent<ModelRenderer>().Model;
		mountPoint = new GameObject()
		{
			WorldPosition = WorldPosition,
			Parent = GameObject,
		};
		worldPanelComponent = mountPoint.AddComponent<Sandbox.WorldPanel>();
		Network.Refresh();
	}

	protected void InitializeRenderScreen()
	{
		worldPanel = worldPanelComponent.GetPanel() as Sandbox.UI.WorldPanel;
		Label = worldPanel.Add.Label( "Wire Screen:", "ds-text" );
		Value = worldPanel.Add.Label( "0", "ds-text" );
		worldPanel.Style.FlexDirection = FlexDirection.Column;
		// Useful for debugging sizes:
		// worldPanel.Style.BorderColor = Color.Red;
		// worldPanel.Style.BorderWidth = 2;
		Label.Style.Width = Model.Bounds.Size.y * 8.5f;
		Label.Style.TextAlign = TextAlign.Center;
		Label.Style.FontSize = Length.Pixels( Model.Bounds.Size.y * 1.25f );
		Label.Style.FontColor = Color.White;
		Value.Style.Width = Model.Bounds.Size.y * 8.5f;
		Value.Style.TextAlign = TextAlign.Center;
		Value.Style.FontSize = Length.Pixels( Model.Bounds.Size.y );
		Value.Style.FontColor = Color.White;

		var modelData = ScreenDatabase.GetValueOrDefault( Model.Name );
		mountPoint.WorldPosition = Transform.World.PointToWorld( modelData.Position );
		mountPoint.WorldRotation = Transform.World.RotationToWorld( modelData.Rotation );
		worldPanelComponent.PanelSize = modelData.Size / Sandbox.UI.WorldPanel.ScreenToWorldScale;
	}

	protected override void OnUpdate()
	{
		if ( !worldPanelComponent.IsValid || !worldPanelComponent.GetPanel().IsValid() ) return;
		if ( worldPanel == null || Label.Style.TextAlign != TextAlign.Center )
		{
			InitializeRenderScreen();
		}
		Label.Text = LabelPrefix;
		Value.Text = valueString;
	}

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
				Position = new Vector3( 0, 0, 2 ),
				Rotation = Rotation.From( new Angles( -90, 0, 0 ) ),
				Size = new Vector2( 50, 30 ),
				ScreenTextureIndex = 1,
			},
			["models/others/monitor/monitor.vmdl"] = new ScreenData()
			{
				Position = new Vector3( -0.4f, -0.1f, 17.25f ),
				Rotation = Rotation.From( new Angles( -0.1f, 0, 0 ) ),
				Size = new Vector2( 37, 21 ),
				ScreenTextureIndex = 1,
			},
		};
	}

	public static Dictionary<string, ScreenData> ScreenDatabase = new();
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
