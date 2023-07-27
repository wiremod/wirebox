using Sandbox;

/* Todo 2023: partial classes don't stretch between assemblies, so this won't work as of being an addon
partial class SandboxPlayer
{
	// basic ugly overlay
	[GameEvent.Client.Frame]
	public void OnFrame()
	{

		var startPos = EyePosition;
		var dir = EyeRotation.Forward;

		var tr = Trace.Ray( startPos, startPos + dir * 200 )
			.Ignore( this )
			.Run();
		if ( tr.Entity is IWireEntity wireEntity )
		{
			var text = wireEntity.GetOverlayText();
			if ( text != "" )
			{
				DebugOverlay.Text( wireEntity.GetOverlayText(), tr.Entity.Position );
			}
		}
	}
}
*/
