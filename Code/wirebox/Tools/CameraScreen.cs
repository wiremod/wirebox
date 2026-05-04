namespace Sandbox.Tools
{
	[Library( "tool_wirecamerascreen", Title = "Camera Screen", Description = "Create a Wire Camera Screen for rendering real time views", Group = "construction" )]
	public partial class CameraScreenTool : BaseSpawnTool
	{
		[Property, Title( "Screen Model" ), ModelProperty( SpawnLists = ["screen"] )]
		public override string SpawnModel { get; set; } = "models/television/flatscreen_tv.vmdl";

		[Property, Title( "Camera Model" ), ModelProperty( SpawnLists = ["camera"] )]
		public static string CameraModel { get; set; } = "camera/camera.vmdl";

		protected override TypeDescription GetSpawnedComponent()
		{
			return TypeLibrary.GetType<WireCameraScreenComponent>();
		}

		private void _loadCloudModel()
		{
			Cloud.Model( "smlp/camera" ); // unreachable but having this in code will force it to be bundled
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
			// todo: add ModelSelector UI for the Camera part
			prop.Model = Model.Load( CameraModel );

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
