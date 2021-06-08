using System;
namespace Sandbox.Tools
{
	public abstract partial class BaseWireTool : BaseTool
	{
		PreviewEntity previewModel;

		abstract protected Type GetEntityType();
		abstract protected string GetModel();
		abstract protected ModelEntity SpawnEntity( TraceResult tr );

		protected override bool IsPreviewTraceValid( TraceResult tr )
		{
			if ( !base.IsPreviewTraceValid( tr ) ) {
				return false;
			}
			if ( tr.Entity.GetType().IsSubclassOf( GetEntityType() ) ) {
				return false;
			}

			return true;
		}

		public override void CreatePreviews()
		{
			if ( TryCreatePreview( ref previewModel, GetModel() ) ) {
				previewModel.RelativeToNormal = false;
			}
		}

		public override void Simulate()
		{
			if ( !Host.IsServer ) {
				return;
			}

			using ( Prediction.Off() ) {
				var input = Owner.Input;

				if ( !input.Pressed( InputButton.Attack1 ) )
					return;

				var startPos = Owner.EyePos;
				var dir = Owner.EyeRot.Forward;

				var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
					.Ignore( Owner )
					.Run();

				if ( !tr.Hit || !tr.Entity.IsValid() ) {
					return;
				}

				CreateHitEffects( tr.EndPos );

				if ( tr.Entity.GetType().IsSubclassOf( GetEntityType() ) ) {
					return;
				}

				var ent = SpawnEntity( tr );
				ent.SetModel( GetModel() );

				var attachEnt = tr.Body.IsValid() ? tr.Body.Entity : tr.Entity;

				if ( attachEnt.IsValid() ) {
					ent.SetParent( tr.Body.Entity, tr.Body.PhysicsGroup.GetBodyBoneName( tr.Body ) );
				}
			}
		}
	}
}
