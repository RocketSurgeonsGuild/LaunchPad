using System.Reflection;

namespace Extensions.Tests.Mapping.Helpers;

public class MethodResult(MethodInfo methodInfo)
{
    public MapResult Map(object mapper, params object[] instances)
    {
        var parameterType = methodInfo.GetParameters()[0].ParameterType;
        var source = Activator.CreateInstance(parameterType);
        var instanceLookup = instances.ToDictionary(x => x.GetType());
        foreach (var property in parameterType
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            if (!instanceLookup.TryGetValue(Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType, out var value))
            {
                continue;
            }
            property.SetValue(source, value);
        }

        return new(source,  methodInfo.Invoke(mapper, [source]));
    }

    public override string ToString()
    {
        return $"{methodInfo.Name}({methodInfo.GetParameters()[0].ParameterType.Name} -> {methodInfo.ReturnType.Name})";
    }
}