using System;
using Sandbox.UI;

namespace Sandbox.Tools
{
	public abstract partial class BaseWireTool : BaseTool
	{
		PreviewEntity previewModel;

		abstract protected Type GetEntityType();
		protected virtual string GetModel()
		{
			var toolCurrent = GetConvarValue( "tool_current", "" );
			return GetConvarValue( $"{toolCurrent}_model" ) ?? "models/citizen_props/coffeemug01.vmdl";
		}
		protected virtual string[] GetSpawnLists()
		{
			return new string[] { GetConvarValue( "tool_current", "" )[5..] };
		}
		abstract protected ModelEntity SpawnEntity( TraceResult tr );

		protected virtual void UpdateEntity( Entity ent ) { }

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
			if ( GetModel() != previewModel.GetModelName() ) {
				previewModel.SetModel( GetModel() );
			}
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
					UpdateEntity( tr.Entity );
					return;
				}

				var ent = SpawnEntity( tr );
				ent.SetModel( GetModel() );

				var attachEnt = tr.Body.IsValid() ? tr.Body.Entity : tr.Entity;

				if ( attachEnt.IsValid() ) {
					ent.SetParent( tr.Body.Entity, tr.Body.PhysicsGroup.GetBodyBoneName( tr.Body ) );
				}

				Sandbox.Hooks.Entities.TriggerOnSpawned( ent, Owner );
			}
		}

		public override void Activate()
		{
			base.Activate();
			if ( Host.IsClient ) {
				var current_tool = GetConvarValue( "tool_current" );
				if ( GetConvarValue( $"{current_tool}_model" ) != null ) {
					var modelSelector = new ModelSelector( GetSpawnLists() );
					SpawnMenu.Instance?.ToolPanel?.AddChild( modelSelector );
				}
			}
		}
	}
}
