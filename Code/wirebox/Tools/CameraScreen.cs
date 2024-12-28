namespace Sandbox.Tools
{
	[Library( "tool_wirecamerascreen", Title = "Wire Camera Screen", Description = "Create a Wire Camera Screen for rendering real time views", Group = "construction" )]
	public partial class CameraScreenTool : BaseSpawnTool
	{
		[ConVar( "tool_wirecamerascreen_model" )]
		public static string _ { get; set; } = "models/television/flatscreen_tv.vmdl";
		[ConVar( "tool_wirecamerascreen_cameramodel" )]
		public static string _2 { get; set; } = "camera/camera.vmdl"; // -> Cloud.Asset( "smlp/camera" );

		protected override TypeDescription GetSpawnedComponent()
		{
			return TypeLibrary.GetType<WireCameraScreenComponent>();
		}
		protected override string[] GetSpawnLists()
		{
			return new string[] { "screen" };
		}

		public override void Activate()
		{
			if ( !IsProxy )
			{
				Description = $"Create a Wire Camera Screen for rendering real time views.\n";
				Description += $"\n{Input.GetButtonOrigin( "attack1" )}: Create Screen";
				Description += $"\n{Input.GetButtonOrigin( "attack2" )}: Create Camera";
			}

			base.Activate();
		}

		protected override void UpdateEntity( GameObject go )
		{
			var screen = go.GetComponent<WireCameraScreenComponent>();
			screen.OnNewModel( screen.GetComponent<ModelRenderer>().Model );
		}

		// BaseSpawnTool handles Primary (default SpawnEntity behaviour) for the screen
		// Secondary spawns a Camera
		public override bool Secondary( SceneTraceResult tr )
		{
			if ( !Input.Pressed( "attack2" ) )
				return false;

			if ( !tr.Hit || !tr.GameObject.IsValid() )
				return false;


			var go = new GameObject()
			{
				WorldPosition = tr.HitPosition,
				WorldRotation = Rotation.LookAt( tr.Normal, tr.Direction ) * Rotation.From( new Angles( 90, 0, 0 ) ),
			};
			var prop = go.AddComponent<Prop>();
			var model = GetConvarValue( "tool_camerascreen_cameramodel", Cloud.Asset( "smlp/camera" ));
			// todo: add ModelSelector UI for the Camera part
			prop.Model = Model.Load( model );

			go.AddComponent<PropHelper>();
			go.AddComponent<WireCameraComponent>();

			go.NetworkSpawn();
			go.Network.SetOrphanedMode( NetworkOrphaned.Host );

			UndoSystem.Add( creator: this.Owner, callback: () =>
			{
				go.Destroy();
				return $"Undid Wire Camera creation";
			}, prop: go );

			return true;
		}
	}
}
