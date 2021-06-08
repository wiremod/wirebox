namespace Sandbox.Tools
{
	[Library( "tool_wire", Title = "Wire", Description = "Wire entities together", Group = "construction" )]
	public partial class WireTool : BaseTool
	{
		private Entity inputEnt;

		public override void Simulate()
		{
			if ( !Host.IsServer )
				return;

			using ( Prediction.Off() ) {
				var input = Owner.Input;
				var startPos = Owner.EyePos;
				var dir = Owner.EyeRot.Forward;

				var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
					.Ignore( Owner )
					.Run();

				if ( !tr.Hit || !tr.Body.IsValid() || !tr.Entity.IsValid() || tr.Entity.IsWorld )
					return;

				if ( input.Pressed( InputButton.Attack1 ) ) {

					if ( !inputEnt.IsValid() ) {
						// stage 1

						if ( tr.Entity is not WireInputEntity wireProp )
							return;
						inputEnt = tr.Entity;
					}
					else {
						// stage 2
						if ( inputEnt is not WireInputEntity wireInputProp )
							return;
						if ( tr.Entity is not WireOutputEntity wireOutputProp )
							return;

						var outputName = wireOutputProp.GetOutputNames()[0];
						var inputName = wireInputProp.GetInputNames()[0];

						wireOutputProp.WireConnect( wireInputProp, outputName, inputName );
						WireOutputEntity.WireTriggerOutput( wireOutputProp, outputName, wireOutputProp.GetOutput( outputName ).value );
						Reset();
					}
				}
				else if ( input.Pressed( InputButton.Attack2 ) ) {
					Reset();
				}
				else if ( input.Pressed( InputButton.Reload ) ) {
					Reset();
				}
				else {
					return;
				}

				CreateHitEffects( tr.EndPos );
			}
		}

		private void Reset()
		{
			inputEnt = null;
		}

		public override void Activate()
		{
			base.Activate();

			Reset();
		}

		public override void Deactivate()
		{
			base.Deactivate();

			Reset();
		}
	}
}
