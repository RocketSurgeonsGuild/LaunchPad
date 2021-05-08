using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.LaunchPad.Functions;
using System.Collections.Generic;
using System.Reflection;

namespace Functions.Tests
{
    internal class TestAssemblyProvider : IAssemblyProvider
    {
        public IEnumerable<Assembly> GetAssemblies() => new[]
        {
            typeof(LaunchPadFunctionStartup).GetTypeInfo().Assembly,
            typeof(TestAssemblyProvider).GetTypeInfo().Assembly
        };
    }
}