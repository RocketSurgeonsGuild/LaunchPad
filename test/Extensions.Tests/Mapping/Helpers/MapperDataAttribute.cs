global using Extensions.Tests.Mapping.Helpers;
using System.Reflection;
using Xunit.Sdk;
using Xunit.v3;

namespace Extensions.Tests.Mapping.Helpers;

public class MapperDataAttribute<TMapper> : DataAttribute
{
    /// <inheritdoc />
    public override ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(MethodInfo testMethod, DisposalTracker disposalTracker) =>
        ValueTask.FromResult<IReadOnlyCollection<ITheoryDataRow>>(
            [
                ..typeof(TMapper)
                 .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                 .Where(method => !method.IsGenericMethodDefinition)
                 .Where(method => method.GetParameters() is [{ ParameterType.IsClass: true }])
                 .Select(method => new MethodResult(method))
            ]
        );

    /// <inheritdoc />
    public override bool SupportsDiscoveryEnumeration() => true;
}
