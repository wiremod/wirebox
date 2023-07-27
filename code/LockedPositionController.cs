namespace Sandbox
{
	public partial class LockedPositionController : WalkController
	{
		private PhysicsBody GroundBody;
		private Vector3 GroundLocalPos;

		public override void Simulate()
		{
			EyeLocalPosition = Vector3.Up * (EyeHeight * Pawn.Scale);
			UpdateBBox();

			EyeLocalPosition += TraceOffset;
			EyeRotation = (Pawn as Player).ViewAngles.ToRotation();

			if ( GroundBody == null )
			{
				(Pawn as Player).Inventory.SetActiveSlot( -1, true );
				var trace = TraceBBox( Position, Position + Velocity * Time.Delta - new Vector3( 0, 0, 5 ) );
				if ( trace.Hit && trace.Body != null )
				{
					GroundBody = trace.Body;
					GroundLocalPos = trace.Body.Transform.PointToLocal( Position );
				}
			}
			if ( GroundBody != null )
			{
				Position = GroundBody.Transform.PointToWorld( GroundLocalPos ) - GroundBody.Velocity * Time.Delta;
				Velocity = GroundBody.Velocity;
			}
		}
	}
}
