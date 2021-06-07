using System;
namespace Sandbox.Tools
{
	[Library( "tool_button", Title = "Button", Description = "Create Buttons!", Group = "construction" )]
	public partial class ButtonTool : BaseTool
	{
		PreviewEntity previewModel;

		protected override bool IsPreviewTraceValid( TraceResult tr )
		{
			if ( !base.IsPreviewTraceValid( tr ) )
				return false;

			if ( tr.Entity is ButtonEntity )
				return false;

			return true;
		}

		public override void CreatePreviews()
		{
			if ( TryCreatePreview( ref previewModel, "models/wirebox/katlatze/button.vmdl" ) ) {
				previewModel.RelativeToNormal = false;
			}
		}

		public override void Simulate()
		{
			if ( !Host.IsServer )
				return;

			using ( Prediction.Off() ) {
				var input = Owner.Input;

				if ( !input.Pressed( InputButton.Attack1 ) )
					return;

				var startPos = Owner.EyePos;
				var dir = Owner.EyeRot.Forward;

				var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
					.Ignore( Owner )
					.Run();

				if ( !tr.Hit )
					return;

				if ( !tr.Entity.IsValid() )
					return;

				CreateHitEffects( tr.EndPos );

				if ( tr.Entity is ButtonEntity )
					return;

				var ent = new ButtonEntity {
					Position = tr.EndPos,
				};

				ent.SetModel( "models/wirebox/katlatze/button.vmdl" );

				var attachEnt = tr.Body.IsValid() ? tr.Body.Entity : tr.Entity;

				if ( attachEnt.IsValid() ) {
					ent.SetParent( tr.Body.Entity, tr.Body.PhysicsGroup.GetBodyBoneName( tr.Body ) );
				}
			}
		}
	}
}
