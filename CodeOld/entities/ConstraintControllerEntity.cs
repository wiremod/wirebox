using Sandbox;
using Sandbox.Physics;
using Sandbox.Tools;
using System;

[Library( "ent_wireconstraintcontroller", Title = "Constraint Controller" )]
public partial class ConstraintControllerEntity : Prop, IWireInputEntity
{
	public PhysicsJoint Joint { get; set; }
	public ConstraintType JointType { get; set; }
	public Func<string> JointCleanup { get; set; }
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();

	public string[] SboxToolAutoTools => new string[] { "tool_constraint" };

	public void WireInitialize()
	{
		this.RegisterInputHandler( "On", ( bool value ) =>
		{
			Joint.EnableLinearConstraint = value;
			Joint.EnableAngularConstraint = value;
		} );
		if ( Joint is SpringJoint spring )
		{
			this.RegisterInputHandler( "Length", ( float value ) =>
			{
				spring.MaxLength = value;
			} );
			this.RegisterInputHandler( "Damping", ( float value ) =>
			{
				spring.SpringLinear = new PhysicsSpring( spring.SpringLinear.Frequency, value );
			} );
			this.RegisterInputHandler( "Strength", ( float value ) =>
			{
				spring.SpringLinear = new PhysicsSpring( value, spring.SpringLinear.Damping );
			} );
			this.RegisterInputHandler( "Retract", ( float value ) =>
			{
				spring.MaxLength -= value;
			} );
			this.RegisterInputHandler( "Extend", ( float value ) =>
			{
				spring.MaxLength += value;
			} );
			// ReferenceMass, EnableLinearConstraint, and EnableAngularConstraint don't appear to do anything todo: maybe as of 2023
		}
		else if ( Joint is FixedJoint weld )
		{
		}
		else if ( Joint is HingeJoint axis )
		{
			this.RegisterInputHandler( "Friction", ( float value ) =>
			{
				axis.Friction = value;
			} );
			// TargetVelocity/TargetAngle sound neat, but don't seem to work? todo: maybe as of 2023
		}
	}

	protected override void OnDestroy()
	{
		if ( JointCleanup != null )
		{
			_ = JointCleanup();
		}
		base.OnDestroy();
	}

	public static void CreateFromTool( Player owner, TraceResult tr, ConstraintType type, PhysicsJoint joint, Func<string> undo )
	{
		var ent = new ConstraintControllerEntity
		{
			Position = tr.EndPosition,
			Rotation = Rotation.LookAt( tr.Normal, owner.EyeRotation.Forward ) * Rotation.From( new Angles( 90, 0, 0 ) ),
			JointType = type,
			Joint = joint,
			JointCleanup = undo,
		};

		if ( tr.Body.IsValid() && !tr.Entity.IsWorld )
		{
			ent.SetParent( tr.Body.GetEntity(), tr.Body.GroupName );
		}

		ent.SetModel( "models/citizen_props/coin01.vmdl" );

		Event.Run( "entity.spawned", ent, owner );
	}
}

