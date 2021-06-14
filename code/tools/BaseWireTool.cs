using System;
namespace Sandbox.Tools
{
	public abstract partial class BaseWireTool : BaseTool
	{
		PreviewEntity previewModel;

		abstract protected Type GetEntityType();
		abstract protected string GetModel();
		abstract protected ModelEntity SpawnEntity( TraceResult tr );

		protected virtual void UpdateEntity(Entity ent) {}

		protected override bool IsPreviewTraceValid( TraceResult tr )
		{
			if ( !base.IsPreviewTraceValid( tr ) ) {
				return false;
			}
			if ( tr.Entity.GetType() == GetEntityType() ) {
				return false;
			}

			return true;
		}

		public override void CreatePreviews()
		{
			if ( TryCreatePreview( ref previewModel, GetModel() ) ) {
				previewModel.RelativeToNormal = true;
				previewModel.RotationOffset = Rotation.From( new Angles( 90, 0, 0 ) );
			}
		}

		public override void Simulate()
		{
			if ( !Host.IsServer ) {
				return;
			}

			using ( Prediction.Off() ) {
				if ( !Input.Pressed( InputButton.Attack1 ) )
					return;

				var startPos = Owner.EyePos;
				var dir = Owner.EyeRot.Forward;

				var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
					.Ignore( Owner )
					.Run();

				if ( !tr.Hit || !tr.Entity.IsValid() ) {
					return;
				}

				CreateHitEffects( tr.EndPos, tr.Normal );

				if ( tr.Entity.GetType() == GetEntityType() ) {
					UpdateEntity(tr.Entity);
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
