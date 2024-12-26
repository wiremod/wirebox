using System;
namespace Sandbox.Tools
{
	[Library( "tool_wirecamerascreen", Title = "Wire Camera Screen", Description = "Create a Wire Camera Screen for rendering real time views", Group = "construction" )]
	public partial class CameraScreenTool : BaseWireTool
	{
		[ConVar.ClientData( "tool_wirecamerascreen_model" )]
		public static string _ { get; set; } = "models/television/flatscreen_tv.vmdl";
		[ConVar.ClientData( "tool_wirecamerascreen_cameramodel" )]
		public static string _2 { get; set; } = "camera/camera.vmdl"; // -> Cloud.Asset( "smlp/camera" );

		protected override Type GetEntityType()
		{
			return typeof( WireCameraScreenEntity );
		}
		protected override string[] GetSpawnLists()
		{
			return new string[] { "screen" };
		}

		public override void Activate()
		{
			if ( Game.IsClient )
			{
				Description = $"Create a Wire Camera Screen for rendering real time views.\n";
				Description += $"\n{Input.GetButtonOrigin( "attack1" )}: Create Screen";
				Description += $"\n{Input.GetButtonOrigin( "attack2" )}: Create Camera";
			}

			base.Activate();
		}

		public override void Simulate()
		{
			base.Simulate(); // handles attack1 (default SpawnEntity behaviour)

			if ( !Game.IsServer )
			{
				return;
			}

			using ( Prediction.Off() )
			{
				if ( !Input.Pressed( "attack2" ) )
					return;

				var tr = DoTrace();

				if ( !tr.Hit || !tr.Entity.IsValid() )
				{
					return;
				}

				CreateHitEffects( tr.EndPosition, tr.Normal );

				var ent = new WireCameraEntity
				{
					Position = tr.EndPosition,
					Rotation = Rotation.LookAt( tr.Normal, tr.Direction ) * Rotation.From( new Angles( 90, 0, 0 ) )
				};
				// todo: add ModelSelector UI for the Camera part
				var model = GetConvarValue( "tool_camerascreen_cameramodel" ) ?? Cloud.Asset( "smlp/camera" );
				ent.SetModel( model );

				Event.Run( "entity.spawned", ent, Owner );
			}
		}
	}
}
