using System.Reflection;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

internal static class StronglyTypedIdHelpers
{
    public static bool IsStronglyTypedId(Type? type)
    {
        return GetStronglyTypedIdType(type) is { };
    }

    public static Type? GetStronglyTypedIdType(Type? type)
    {
        if (type?.GetMember("New", BindingFlags.Static | BindingFlags.Public).FirstOrDefault() is MethodInfo
         && type.GetMember("Empty", BindingFlags.Static | BindingFlags.Public).FirstOrDefault() is FieldInfo { }
         && type.GetMember("Value", BindingFlags.Instance | BindingFlags.Public).FirstOrDefault() is PropertyInfo propertyInfo)
            return propertyInfo.PropertyType;

        return null;
    }
}

internal class StronglyTypedIdSchemaFilter : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (StronglyTypedIdHelpers.GetStronglyTypedIdType(context.JsonTypeInfo.Type) is not { } type) return Task.CompletedTask;
        return Task.CompletedTask;
    }
}
