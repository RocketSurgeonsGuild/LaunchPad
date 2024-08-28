global using Extensions.Tests.Mapping.Helpers;
using System.Reflection;
using Xunit.Sdk;

namespace Extensions.Tests.Mapping.Helpers;

public class MapperDataAttribute<TMapper> : DataAttribute
{
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        var methods = typeof(TMapper).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (var method in methods)
        {
            if (method.IsGenericMethodDefinition) continue;
            var parameters = method.GetParameters();
            if (parameters is not [{ ParameterType.IsClass: true, },]) continue;
            yield return [new MethodResult(method),];
        }
    }
}