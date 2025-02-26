using System.Reflection;

namespace Extensions.Tests.Mapping.Helpers;

public class MethodResult(MethodInfo methodInfo) : TheoryDataRowBase
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

        return new(source, methodInfo.Invoke(mapper, [source,]));
    }

    public IEnumerable<MapResult> MapEach(object mapper, params object[] instances)
    {
        var parameterType = methodInfo.GetParameters()[0].ParameterType;
        var instanceLookup = instances.ToLookup(x => Nullable.GetUnderlyingType(x.GetType()) ?? x.GetType());
        var propertyLookup = parameterType
                            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                            .ToDictionary(z => Nullable.GetUnderlyingType(z.PropertyType) ?? z.PropertyType);

        return instanceLookup
              .Join(
                   propertyLookup,
                   z => z.Key,
                   z => z.Key,
                   (instances, properties) =>
                   {
                       var propertyInfo = properties.Value;
                       return instances.Select(
                           instance =>
                           {
                               var source = Activator.CreateInstance(parameterType);
                               propertyInfo.SetValue(source, instance);
                               return source;
                           }
                       );
                   }
               )
              .SelectMany(sources => sources, (_, source) => new MapResult(source, methodInfo.Invoke(mapper, [source,])));
    }

    public override string ToString()
    {
        return $"{methodInfo.Name}({methodInfo.GetParameters()[0].ParameterType.Name} -> {methodInfo.ReturnType.Name})";
    }

    /// <inheritdoc />
    protected override object?[] GetData() => [this];
}
