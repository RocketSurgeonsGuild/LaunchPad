using System.Runtime.CompilerServices;

namespace Rocket.Surgery.LaunchPad.Testing;

static class ModuleInitializer
{
    #pragma warning disable CA2255
    [ModuleInitializer]
    #pragma warning restore CA2255
    internal static void Initialize()
    {
        Environment.SetEnvironmentVariable("RSG__HOSTTYPE", "UnitTest");
    }
}
