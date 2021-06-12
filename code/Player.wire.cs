using Sandbox;

partial class SandboxPlayer
{
	// basic ugly overlay
	[Event.Frame]
	public void OnFrame()
	{
		var startPos = EyePos;
		var dir = EyeRot.Forward;

		var tr = Trace.Ray( startPos, startPos + dir * 200 )
			.Ignore( this )
			.Run();
		if ( tr.Entity is IWireEntity wireEntity ) {
			var text = wireEntity.GetOverlayText();
			if ( text != "" ) {
				DebugOverlay.Text( tr.Entity.Position, wireEntity.GetOverlayText() );
			}
		}
	}
}
