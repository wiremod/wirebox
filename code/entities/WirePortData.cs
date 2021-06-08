using System;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox
{
	public class WirePortData
	{
		public Dictionary<string, Action<object>> inputHandlers { get; } = new Dictionary<string, Action<object>>();
		public Dictionary<string, WireInput> inputs = new Dictionary<string, WireInput>();
		public Dictionary<string, WireOutput> outputs = new Dictionary<string, WireOutput>();

	}

	public interface IWireEntity
	{
		public WirePortData WirePorts { get; }
	}
}
