namespace SandboxPlus;

// Temporarily putting this attribute here instead of in SandboxPlus, because Libraries (like WireLib) can't reference Games, only vice versa
[AttributeUsage( AttributeTargets.Method, Inherited = false )]
public class GameInitAttribute : Attribute
{
	public bool HostOnly { get; }
	public GameInitAttribute( bool HostOnly = false )
	{
		this.HostOnly = HostOnly;
	}
}
