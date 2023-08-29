using System;
namespace Sandbox.Tools
{
	[Library( "tool_wireforcer", Title = "Wire Forcer", Description = "Create a Wire Forcer for pushing/pulling props", Group = "construction" )]
	public partial class ForcerTool : BaseWireTool
	{
		[ConVar.ClientData( "tool_wireforcer_model" )]
		public static string _ { get; set; } = "models/wirebox/katlatze/apc.vmdl";
		[ConVar.ClientData( "tool_wireforcer_length" )]
		public static float _2 { get; set; } = 100f;

		protected override Type GetEntityType()
		{
			return typeof( WireRangerEntity );
		}
		protected override ModelEntity SpawnEntity( TraceResult tr )
		{
			return new WireForcerEntity
			{
				Position = tr.EndPosition,
				Rotation = Rotation.LookAt( tr.Normal, tr.Direction ) * Rotation.From( new Angles( 90, 0, 0 ) ),
				Length = float.Parse( GetConvarValue( "tool_wireforcer_length" ) ),
			};
		}
		protected override string[] GetSpawnLists()
		{
			return new string[] { "ranger", "controller" };
		}

		public override void CreateToolPanel()
		{
			if ( Game.IsClient )
			{
				var toolConfigUi = new ForcerToolConfig();
				SpawnMenu.Instance?.ToolPanel?.AddChild( toolConfigUi );
			}
		}
	}
}
