using System.Threading;

namespace Sandbox
{
	public partial class WireCable
	{
		private static List<WireCable> WireCables = [];
		private GameObject ent1;
		private GameObject ent2;

		public LegacyParticleSystem Particle { get; }

		public WireCable( LegacyParticleSystem particle, GameObject ent1, GameObject ent2 )
		{
			Particle = particle;
			this.ent1 = ent1;
			this.ent2 = ent2;
			WireCables.Add( this );

			if ( ent1.GetComponent<Prop>() is var propHelper && propHelper.IsValid() )
			{
				propHelper.OnComponentDestroy += Destroy;
			}

			if ( ent2.GetComponent<Prop>() is var propHelper2 && propHelper2.IsValid() )
			{
				propHelper2.OnComponentDestroy += Destroy;
			}
		}

		// In 2023's engine, it was very unreliable killing the visual cables when the entities were destroyed. It might be better now?
		private static CancellationTokenSource cancellationTokenSource;

		// [Event( "game.init" )]
		// [Event( "package.mounted" )]
		// [Event.Hotload]
		public static async void InitCleanupTimer()
		{
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
					wireCable.Destroy();
				}
			}
		}
		public static void StopCleanupTimer()
		{
			cancellationTokenSource?.Cancel();
		}

		public void Destroy()
		{
			Particle?.Destroy();
			WireCables.Remove( this );
		}
	}
}
