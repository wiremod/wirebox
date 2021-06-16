using System;
namespace Sandbox.Tools
{
	[Library( "tool_wiregyroscope", Title = "Wire Gyroscope", Description = "Create a Wire Gyroscope for retrieving rotation data", Group = "construction" )]
	public partial class GyroscopeTool : BaseWireTool
	{
		protected override Type GetEntityType()
		{
			return typeof( WireGyroscopeEntity );
		}
		protected override string GetModel()
		{
			return "models/citizen_props/icecreamcone01.vmdl";
		}
		protected override ModelEntity SpawnEntity( TraceResult tr )
		{
			return new WireGyroscopeEntity {
				Position = tr.EndPos,
				Rotation = Rotation.LookAt( tr.Normal, tr.Direction ) * Rotation.From( new Angles( 90, 0, 0 ) )
			};
		}
	}
}
