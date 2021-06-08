using System;
namespace Sandbox.Tools
{
	[Library( "tool_numpadinput", Title = "Numpad Input", Description = "Create Numpad Inputs!", Group = "construction" )]
	public partial class NumpadInputTool : BaseWireTool
	{
		protected override Type GetEntityType()
		{
			return typeof( NumpadInputEntity );
		}
		protected override string GetModel()
		{
			return "models/wirebox/katlatze/button.vmdl";
		}
		protected override ModelEntity SpawnEntity( TraceResult tr )
		{
			return new NumpadInputEntity {
				Position = tr.EndPos,
				WireOwner = Owner,
			};
		}
	}
}
