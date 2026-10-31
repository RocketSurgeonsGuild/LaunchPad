global using Extensions.Tests.Mapping.Helpers;
using System.Reflection;

namespace Extensions.Tests.Mapping.Helpers;

/// <summary>
///     The test data of the mapper tests: one <see cref="MethodResult" /> for every mapping method a mapper declares.
///     A test points at it through a method data source on its own class, because the generated test registration cannot name
///     the private nested mapper type (as it could with a generic data source attribute).
/// </summary>
public static class MapperData
{
    public static IEnumerable<Func<MethodResult>> Methods<TMapper>() =>
        typeof(TMapper)
           .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           .Where(method => !method.IsGenericMethodDefinition)
           .Where(method => method.GetParameters() is [{ ParameterType.IsClass: true }])
           .Select(method => (Func<MethodResult>)( () => new MethodResult(method) ));
}
