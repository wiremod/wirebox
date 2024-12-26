using System;
namespace Sandbox.Tools
{
	[Library( "tool_wireweight", Title = "Wire Weight", Description = "Create a Wire adjustable Weight. Can be negative, like a balloon!", Group = "construction" )]
	public partial class WireWeightTool : BaseWireTool
	{
		[ConVar.ClientData( "tool_wireweight_model" )]
		public static string _ { get; set; } = "models/citizen_props/icecreamcone01.vmdl";

		protected override Type GetEntityType()
		{
			return typeof( WireWeightEntity );
		}
		protected override string[] GetSpawnLists()
		{
			return new string[] { "weight", "controller" };
		}
	}
}
