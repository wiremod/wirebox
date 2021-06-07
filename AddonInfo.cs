using System;
using System.Collections.Generic;
using MinimalExtended;
using Sandbox;

namespace Wirebox
{
  [Library("wirebox")]
  public class AddonInfo : IAddonInfo
  {
    public string Name => "WireBox";

    public string Description => "Wiremod for S&Box";

    public string Author => "Wireteam";

    public double Version => 0.1;

    public List<AddonDependency> Dependencies => new()
    {
      new AddonDependency()
      {
        Name = "Sandbox",
        MinVersion = 1.0
      }
    };

    // No main class as it should be instantiated by the Entities/Tools (so far)
    public Type MainClass => null;
  }
}
