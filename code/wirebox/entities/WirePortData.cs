using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox
{
	public class WirePortData
	{
		public Dictionary<string, Action<object>> inputHandlers { get; } = new Dictionary<string, Action<object>>();
		public Dictionary<string, WireInput> inputs = new Dictionary<string, WireInput>();
		public Dictionary<string, WireOutput> outputs = new Dictionary<string, WireOutput>();

		public int outputExecutions = 0;
		public int outputExecutionsTick = 0;
	}

	public interface IWireEntity
	{
		public WirePortData WirePorts { get; }

		public virtual string GetOverlayText() { return ""; }
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

		[Event( "hotloaded" )]
		public static async void InitCleanupTimer()
		{
			if ( Host.IsClient ) {
				return;
			}
			StopCleanupTimer();
			cancellationTokenSource = new CancellationTokenSource();
			try {
				while ( true ) {
					CleanupOrphanedCables();
					await Task.Delay( 1500, cancellationTokenSource.Token );
				}
			}
			catch ( TaskCanceledException ) {
				return;
			}
		}

		private static void CleanupOrphanedCables()
		{
			foreach ( var wireCable in WireCables.Reverse<WireCable>() ) {
				if ( wireCable?.ent1 == null || !wireCable.ent1.IsValid()
					|| wireCable?.ent2 == null || !wireCable.ent2.IsValid() ) {
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
