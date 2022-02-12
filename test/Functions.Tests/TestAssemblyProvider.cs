using System.Reflection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.LaunchPad.Functions;

namespace Functions.Tests;

internal class TestAssemblyProvider : IAssemblyProvider
{
    public IEnumerable<Assembly> GetAssemblies()
    {
        return new[]
        {
            typeof(LaunchPadFunctionStartup).GetTypeInfo().Assembly,
            typeof(TestAssemblyProvider).GetTypeInfo().Assembly
        };
    }
}
