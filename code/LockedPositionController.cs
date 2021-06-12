namespace Sandbox
{
	public partial class LockedPositionController : WalkController
	{
		private PhysicsBody GroundBody;
		private Vector3 GroundLocalPos;

		public override void Simulate()
		{
			EyePosLocal = Vector3.Up * (EyeHeight * Pawn.Scale);
			UpdateBBox();

			EyePosLocal += TraceOffset;
			EyeRot = Input.Rotation;

			if ( GroundBody == null ) {
				var trace = TraceBBox( Position, Position + Velocity * Time.Delta - new Vector3( 0, 0, 5 ) );
				if ( trace.Hit && trace.Body != null ) {
					GroundBody = trace.Body;
					GroundLocalPos = trace.Body.Transform.PointToLocal( Position );
				}
			}
			if ( GroundBody != null ) {
				Position = GroundBody.Transform.PointToWorld( GroundLocalPos ) - GroundBody.Velocity * Time.Delta;
				Velocity = GroundBody.Velocity;
			}
		}
	}
}
