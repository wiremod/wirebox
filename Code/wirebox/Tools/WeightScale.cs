namespace Sandbox.Tools
{
	[Library( "tool_wireweightscale", Title = "Weight Scale", Description = "Create a Wire Scale that measures the weight of props sitting on it.", Group = "construction" )]
	public partial class WireWeightScaleTool : BaseSpawnTool
	{
		[Property, Title( "Model" ), ModelProperty( SpawnLists = ["weightscale"] )]
		public override string SpawnModel { get; set; } = "models/sbox_props/pallet/pallet.vmdl"; // -> Cloud.Asset( "facepunch.pallet" );

		protected override TypeDescription GetSpawnedComponent()
		{
			return TypeLibrary.GetType<WireWeightScaleComponent>();
		}
		private void unused()
		{
			Cloud.Model( "facepunch.pallet" ); // this is here to download the model, so we can set it as the default
		}
	}
}
