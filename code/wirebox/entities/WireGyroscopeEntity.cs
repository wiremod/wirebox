﻿using Sandbox;

[Library( "ent_wiregyroscope", Title = "Wire Gyroscope" )]
public partial class WireGyroscopeEntity : Prop, WireOutputEntity, IPhysicsUpdate
{
	WirePortData IWireEntity.WirePorts { get; } = new WirePortData();
	public string[] WireGetOutputs()
	{
		return new string[] { "Pitch", "Yaw", "Roll", "Angle", "Rotation" };
	}

	public void OnPostPhysicsStep( float dt )
	{
		if ( !this.IsValid() )
			return;

		var outputs = ((IWireEntity)this).WirePorts.outputs;
		if ( !outputs.ContainsKey( "Angle" ) ) {
			return;
		}
		var angle = Rotation.Angles();
		if ( outputs["Angle"].value is Angles oldValue && oldValue.Equals( angle ) ) {
			return;
		}
		this.WireTriggerOutput( "Pitch", angle.pitch );
		this.WireTriggerOutput( "Yaw", angle.yaw );
		this.WireTriggerOutput( "Roll", angle.roll );
		this.WireTriggerOutput( "Angle", angle );
		this.WireTriggerOutput( "Rotation", Rotation );
	}
}