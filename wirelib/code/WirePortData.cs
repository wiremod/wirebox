using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox
{
	public class WirePortData
	{
		public bool inputsInitialized = false;
		public Dictionary<string, Action<object>> inputHandlers { get; } = new Dictionary<string, Action<object>>();
		public Dictionary<string, WireInput> inputs = new Dictionary<string, WireInput>();
		public Dictionary<string, WireOutput> outputs = new Dictionary<string, WireOutput>();
	}

	public interface IWireEntity
	{
		public WirePortData WirePorts { get; }

		public virtual string GetOverlayText() { return ""; }

		public static object GetDefaultValueFromType( string type )
		{
			if ( type == "bool" )
				return false;
			else if ( type == "int" )
				return 0;
			else if ( type == "float" )
				return 0.0f;
			else if ( type == "string" )
				return "";
			else if ( type == "vector3" )
				return Vector3.Zero;
			else if ( type == "angle" )
				return Angles.Zero;
			else if ( type == "rotation" )
				return Rotation.Identity;

			return false;
		}
	}

	public partial class WireCable
	{
		private static List<WireCable> WireCables = new();
		private Entity ent1;
		private Entity ent2;

		public Particles Particle { get; }

		public WireCable( string model, Entity ent1, Entity ent2 )
		{
			Particle = Particles.Create( model );
			this.ent1 = ent1;
			this.ent2 = ent2;
			WireCables.Add( this );
		}

		private static CancellationTokenSource cancellationTokenSource;

		[Event( "game.init" )]
		[Event( "package.mounted" )]
		[Event.Hotload]
		public static async void InitCleanupTimer()
		{
			if ( Game.IsClient )
			{
				return;
			}
			StopCleanupTimer();
			cancellationTokenSource = new CancellationTokenSource();
			try
			{
				while ( true )
				{
					CleanupOrphanedCables();
					await Task.Delay( 1500, cancellationTokenSource.Token );
				}
			}
			catch ( TaskCanceledException )
			{
				return;
			}
		}

		private static void CleanupOrphanedCables()
		{
			foreach ( var wireCable in WireCables.Reverse<WireCable>() )
			{
				if ( wireCable?.ent1 == null || !wireCable.ent1.IsValid()
					|| wireCable?.ent2 == null || !wireCable.ent2.IsValid() )
				{
					wireCable.Destroy( true );
				}
			}
		}
		public static void StopCleanupTimer()
		{
			cancellationTokenSource?.Cancel();
		}

		public void Destroy( bool immediately )
		{
			Particle?.Destroy( immediately );
			WireCables.Remove( this );
		}
	}
}
