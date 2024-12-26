using System;
namespace Sandbox.Tools
{
	[Library( "tool_wireweightscale", Title = "Wire Weight Scale", Description = "Create a Wire Scale that measures the weight of props sitting on it.", Group = "construction" )]
	public partial class WireWeightScaleTool : BaseWireTool
	{
		[ConVar.ClientData( "tool_wireweightscale_model" )]
		public static string _ { get; set; } = "models/sbox_props/pallet/pallet.vmdl"; // -> Cloud.Asset( "facepunch.pallet" );

		protected override Type GetEntityType()
		{
			return typeof( WireWeightScaleEntity );
		}
		protected override string[] GetSpawnLists()
		{
			return new string[] { "weightscale" };
		}
		private void unused()
		{
			Cloud.Model( "facepunch.pallet" ); // this is here to download the model, so we can set it as the default
		}
	}
}
