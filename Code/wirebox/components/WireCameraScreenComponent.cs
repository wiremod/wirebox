[Library( "ent_wirecamerascreen", Title = "Screen - Camera" )]
public partial class WireCameraScreenComponent : BaseWireInputComponent
{
	[Sync]
	private WireCameraComponent CameraEntity { get; set; } = null;
	[Sync]
	private int FPS { get; set; } = 20;

	private Texture texture;
	private SceneCustomObject RenderObject;
	private Vector2 Size = new Vector2( 500, 300 );

	public override void WireInitialize()
	{
		this.RegisterInputHandler( "Camera", ( GameObject ent ) =>
		{
			if ( !ent.IsValid() || ent.GetComponent<WireCameraComponent>() is not WireCameraComponent cam )
			{
				CameraEntity = null;
				return;
			}
			CameraEntity = cam;
		} );
		this.RegisterInputHandler( "FPS", ( int val ) =>
		{
			FPS = Math.Clamp( val, 1, 60 );
		}, FPS );
	}

	private TimeSince timeSinceLastRender = 0;
	protected override void OnPreRender()
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

		// todo: could consider caching the Texture between multiple screens of the same size
		Graphics.RenderToTexture( CameraEntity.GetSceneCamera(), texture );
	}

	protected override void OnEnabled()
	{
		base.OnEnabled();

		RenderObject = new SceneCustomObject( Scene.SceneWorld )
		{
			RenderOverride = OnRender,
			RenderingEnabled = false,
		};
		OnNewModel( GetComponent<ModelRenderer>().Model );
	}

	public void OnNewModel( Model model )
	{
		var SceneObject = GetComponent<ModelRenderer>().SceneObject;
		if ( SceneObject.IsValid() )
		{
			var modelData = WireDigitalScreenComponent.ScreenDatabase.GetValueOrDefault( model.Name, new ScreenData() { Size = new Vector2( 50, 50 ) } );
			Size = modelData.Size / Sandbox.UI.WorldPanel.ScreenToWorldScale; // render correct aspect ratio of screen, but larger so it looks good up close

			texture?.Dispose();
			texture = Texture.CreateRenderTarget()
					 .WithSize( Size )
					 .WithScreenFormat()
					 .WithDynamicUsage()
					 .Create();

			// default to showing self
			// kinda goofy looking, but helps self-demonstrate the usage of the screen. Alternatively could just be blank.
			Scene.Camera.RenderToTexture( texture );

			SceneObject.Batchable = false;
			if ( SceneObject.Attributes.GetTexture( "screen" ) != null )
			{
				Log.Info( "CameraScreen: Model has a 'screen' attribute to override directly" );
				SceneObject.Attributes.Set( "screen", texture );
			}
			else
			{
				var material = Material.Create( "wire_camerascreen_rendertexture", "simple" );
				material.Set( "Color", texture );
				material.Set( "Normal", Texture.Transparent ); // unsure how to capture a Depth texture from Graphics.RenderToTexture (or a SceneCamera)

				var mats = model.Materials.ToList();
				for ( int i = 0; i < mats.Count; i++ )
				{
					mats[i].Attributes.Set( "materialIndex" + i, 1 );
				}
				GetComponent<ModelRenderer>().SetMaterialOverride( material, "materialIndex" + modelData.ScreenTextureIndex );
				SceneObject.SetMaterialOverride( material, "materialIndex" + modelData.ScreenTextureIndex, 1 );
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		RenderObject?.Delete();
		RenderObject = null;
		texture?.Dispose();
	}

	private void OnRender( SceneObject sceneObject )
	{
		Graphics.RenderTarget = RenderTarget.From( texture );
		Graphics.Attributes.SetCombo( "D_WORLDPANEL", 0 );
		Graphics.Viewport = new Rect( 0, Size );
		Graphics.Clear();
		Graphics.RenderTarget = null;
	}
}
