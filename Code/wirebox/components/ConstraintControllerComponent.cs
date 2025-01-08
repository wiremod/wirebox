using Sandbox.Physics;
using Sandbox.Tools;

[Library( "ent_wireconstraintcontroller", Title = "Constraint Controller" )]
public partial class ConstraintControllerComponent : BaseWireInputComponent
{
	public Joint Joint { get; set; }
	public ConstraintType JointType { get; set; }
	public Func<string> JointCleanup { get; set; }

	public string[] SboxToolAutoTools => new string[] { "tool_constraint" };

	public override void WireInitialize()
	{
		this.RegisterInputHandler( "On", ( bool value ) =>
		{
			if ( value && !Joint.Enabled && Joint is Sandbox.FixedJoint weld )
			{
				Joint.Attachment = Joint.AttachmentMode.Auto; // reset the position to wherever they currently are
			}
			Joint.Enabled = value;
		}, Joint.Enabled );

		if ( Joint is Sandbox.SpringJoint spring )
		{
			this.RegisterInputHandler( "Length", ( float value ) =>
			{
				spring.MaxLength = value;
			}, spring.MaxLength );
			this.RegisterInputHandler( "Damping", ( float value ) =>
			{
				spring.Damping = value;
			}, spring.Damping );
			this.RegisterInputHandler( "Strength", ( float value ) =>
			{
				spring.Frequency = value;
			}, spring.Frequency );
			this.RegisterInputHandler( "Retract", ( float value ) =>
			{
				spring.MaxLength -= value;
			} );
			this.RegisterInputHandler( "Extend", ( float value ) =>
			{
				spring.MaxLength += value;
			} );
		}
		else if ( Joint is Sandbox.FixedJoint weld )
		{
		}
		else if ( Joint is Sandbox.HingeJoint axis )
		{
			this.RegisterInputHandler( "Friction", ( float value ) =>
			{
				axis.Friction = value;
			}, axis.Friction );
			this.RegisterInputHandler( "TargetAngle", ( float value ) =>
			{
				axis.Motor = Sandbox.HingeJoint.MotorMode.TargetAngle;
				axis.Fequency = 5f; // lol typo in API
				axis.TargetAngle = Angles.NormalizeAngle( value );
			}, axis.TargetAngle );
			// this.RegisterInputHandler( "TargetVelocity", ( float value ) =>
			// {
			// this doesn't seem to work, even if I set MaxTorque very high?
			// 	axis.Motor = Sandbox.HingeJoint.MotorMode.TargetVelocity;
			// 	axis.TargetVelocity = value;
			// } );
		}
		else if ( Joint is Sandbox.BallJoint ballSocket )
		{
			this.RegisterInputHandler( "Friction", ( float value ) =>
			{
				ballSocket.Friction = value;
			}, ballSocket.Friction );
			// todo: SwingLimit and TwistLimit look interesting
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

	public static void CreateFromTool( Player owner, SceneTraceResult tr, ConstraintType type, Joint joint, Func<string> undo )
	{
		var go = new GameObject()
		{
			WorldPosition = tr.HitPosition,
			WorldRotation = Rotation.LookAt( tr.Normal, tr.Direction ) * Rotation.From( new Angles( 90, 0, 0 ) ),
		};
		var prop = go.AddComponent<Prop>();
		prop.Model = Model.Load( "models/wirebox/katlatze/apc.vmdl" );

		var propHelper = go.GetOrAddComponent<PropHelper>();
		var constraintController = go.AddComponent<ConstraintControllerComponent>();
		constraintController.Joint = joint;
		constraintController.JointType = type;
		constraintController.JointCleanup = undo;
		propHelper.Weld( tr.GameObject, noCollide: true, toBone: tr.Bone );

		UndoSystem.Add( creator: owner, callback: () =>
		{
			go.Destroy();
			return $"Undid Constraint Controller creation";
		}, prop: go );

		go.NetworkSpawn();
		go.Network.SetOrphanedMode( NetworkOrphaned.Host );
		Sandbox.Events.IPropSpawnedEvent.Post( x => x.OnSpawned( prop ) );
	}
	[SandboxPlus.GameInit( HostOnly: false )]
	public static void InitConstraintTool()
	{
		SandboxPlus.Tools.ConstraintTool.CreateWireboxConstraintController = CreateFromTool;
	}
}
