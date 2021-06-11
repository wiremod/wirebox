using System;
using System.Collections.Generic;
using MinimalExtended;
using Sandbox;

namespace Wirebox
{
	[Library( "wirebox" )]
	public class AddonInfo : BaseAddonInfo
	{
		public override string Name => "WireBox";

		public override string Description => "Wiremod for S&Box";

		public override string Author => "Wireteam";

		public override double Version => 0.1;

		public override List<AddonDependency> Dependencies => new()
		{
			new AddonDependency() {
				Name = "Sandbox",
				MinVersion = 1.0
			}
		};

		public class WireboxAddon : AddonClass<AddonInfo> { }
	}
}
