using System;
using System.Linq;
using Sandbox;

[Library( "ent_wirecamerascreen", Title = "Screen - Camera" )]
public partial class WireCameraScreenEntity : Prop, IWireInputEntity
{
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();

	[Net]
	private WireCameraEntity CameraEntity { get; set; } = null;
	[Net]
	private int FPS { get; set; } = 20;

	private Texture Texture;
	private SceneCustomObject RenderObject;
	private Vector2 Size = new Vector2( 500, 300 );

	public void WireInitialize()
	{
		this.RegisterInputHandler( "Camera", ( Entity ent ) =>
		{
			if ( !ent.IsValid() || ent is not WireCameraEntity ) // todo check is camera
			{
				CameraEntity = null;
				return;
			}
			CameraEntity = ent as WireCameraEntity;
		} );
		this.RegisterInputHandler( "FPS", ( int val ) =>
		{
			FPS = Math.Clamp( val, 1, 60 );
		}, FPS );
	}

	private TimeSince timeSinceLastRender = 0;
	[GameEvent.Client.Frame]
	public void OnFrame()
	{
		if ( !CameraEntity.IsValid() )
		{
			return;
		}
		// only render every few frames to save FPS
		if ( timeSinceLastRender < (1.0f / FPS) )
		{
			return;
		}
		timeSinceLastRender = 0;

		// Showing the local player seems to be broken as of https://github.com/sboxgame/issues/issues/3147
		// todo: could consider caching the Texture between multiple screens of the same size
		Graphics.RenderToTexture( CameraEntity.GetSceneCamera(), Texture );
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		RenderObject = new SceneCustomObject( Game.SceneWorld )
		{
			RenderOverride = OnRender
		};
	}

	public override void OnNewModel( Model model )
	{
		base.OnNewModel( model );

		if ( SceneObject.IsValid() )
		{
			var modelData = WireDigitalScreenEntity.ScreenDatabase.GetValueOrDefault( Model.Name, new ScreenData() { Size = new Vector2( 50, 50 ) } );
			Size = modelData.Size * 20; // render correct aspect ratio of screen, but larger so it looks good up close

			Texture = Texture.CreateRenderTarget()
					 .WithSize( Size )
					 .WithScreenFormat()
					 .WithScreenMultiSample()
					 .Create();

			// kinda goofy looking, but helps self-demonstrate the usage of the screen. Alternatively could just be blank.
			Graphics.RenderToTexture( Camera.Main, Texture );

			SceneObject.Batchable = false;
			if ( SceneObject.Attributes.GetTexture( "screen" ) != null )
			{
				Log.Info( "CameraScreen: Model has a 'screen' attribute to override directly" );
				SceneObject.Attributes.Set( "screen", Texture );
			}
			else
			{
				var material = Material.Create( "wire_camerascreen_rendertexture", "simple" );
				material.Set( "Color", Texture );
				material.Set( "Normal", Texture.Transparent ); // unsure how to capture a Depth texture from Graphics.RenderToTexture (or a SceneCamera)

				var mats = Model.Materials.ToList();
				for ( int i = 0; i < mats.Count; i++ )
				{
					mats[i].Attributes.Set( "materialIndex" + i, 1 );
				}
				SetMaterialOverride( material, "materialIndex" + modelData.ScreenTextureIndex );
				SceneObject.SetMaterialOverride( material, "materialIndex" + modelData.ScreenTextureIndex, 1 );
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		RenderObject?.Delete();
		RenderObject = null;
	}

	private void OnRender( SceneObject sceneObject )
	{
		if ( sceneObject != SceneObject )
		{
			return;
		}
		Graphics.RenderTarget = RenderTarget.From( Texture );
		Graphics.Attributes.SetCombo( "D_WORLDPANEL", 0 );
		Graphics.Viewport = new Rect( 0, Size );
		Graphics.Clear();
		Graphics.RenderTarget = null;
	}
}

