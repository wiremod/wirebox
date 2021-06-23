using Sandbox;
using Sandbox.Joints;
using Sandbox.Tools;
using System;

[Library( "ent_constraintcontroller", Title = "Constraint Controller" )]
public partial class ConstraintControllerEntity : Prop, WireInputEntity
{
	public object Joint { get; set; }
	public ConstraintType JointType { get; set; }
	public Func<string> JointCleanup { get; set; }
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();

	public void WireInitialize()
	{
		if ( Joint is SpringJoint spring ) {
			this.RegisterInputHandler( "Length", ( float value ) => {
				spring.RestLengthMax = value;
			} );
			this.RegisterInputHandler( "Damping", ( float value ) => {
				spring.DampingRatio = value;
			} );
			this.RegisterInputHandler( "Strength", ( float value ) => {
				spring.Frequency = value;
			} );
			this.RegisterInputHandler( "Retract", ( float value ) => {
				spring.RestLengthMax -= value;
			} );
			this.RegisterInputHandler( "Extend", ( float value ) => {
				spring.RestLengthMax += value;
			} );
			// ReferenceMass, EnableLinearConstraint, and EnableAngularConstraint don't appear to do anything
		}
		else if ( Joint is WeldJoint weld ) {
			this.RegisterInputHandler( "On", ( bool value ) => {
				weld.EnableLinearConstraint = value;
				weld.EnableAngularConstraint = value;
			} );
		}
		else if ( Joint is RevoluteJoint axis ) {
			this.RegisterInputHandler( "Friction", ( float value ) => {
				axis.MotorFriction = value;
			} );
			// TargetVelocity/TargetAngle sound neat, but don't seem to work?
		}
	}

	protected override void OnDestroy()
	{
		if ( JointCleanup != null ) {
			_ = JointCleanup();
		}
		base.OnDestroy();
	}

	public static void CreateFromTool( Player owner, TraceResult tr, ConstraintType type, object joint, Func<string> undo )
	{
		var ent = new ConstraintControllerEntity {
			Position = tr.EndPos,
			Rotation = Rotation.LookAt( tr.Normal, owner.EyeRot.Forward ) * Rotation.From( new Angles( 90, 0, 0 ) ),
			JointType = type,
			Joint = joint,
			JointCleanup = undo,
		};

		if ( tr.Body.IsValid() && !tr.Entity.IsWorld ) {
			ent.SetParent( tr.Entity, tr.Body.PhysicsGroup.GetBodyBoneName( tr.Body ) );
		}

		ent.SetModel( "models/citizen_props/coin01.vmdl" );

		Sandbox.Hooks.Entities.TriggerOnSpawned( ent, owner );
	}
}

