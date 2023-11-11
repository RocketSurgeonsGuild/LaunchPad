using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

internal static class StronglyTypedIdHelpers
{
    [RequiresUnreferencedCode("DynamicBehavior is incompatible with trimming.")]
    public static bool IsStronglyTypedId(Type? type)
    {
        return GetStronglyTypedIdType(type) is { };
    }

    [RequiresUnreferencedCode("DynamicBehavior is incompatible with trimming.")]
    public static Type? GetStronglyTypedIdType(Type? type)
    {
        if (type?.GetMember("New", BindingFlags.Static | BindingFlags.Public).FirstOrDefault() is MethodInfo
         && type.GetMember("Empty", BindingFlags.Static | BindingFlags.Public).FirstOrDefault() is FieldInfo { }
         && type.GetMember("Value", BindingFlags.Instance | BindingFlags.Public).FirstOrDefault() is PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType;
        }

        return null;
    }
}

internal class StronglyTypedIdSchemaFilter : ISchemaFilter
{
    // TODO: Make a source generator for this to work without generics
    [RequiresUnreferencedCode("DynamicBehavior is incompatible with trimming.")]
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (StronglyTypedIdHelpers.GetStronglyTypedIdType(context.Type) is not { } type) return;
        schema.Properties.Clear();
        var s2 = context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository, context.MemberInfo, context.ParameterInfo);
        schema.Format = s2.Format;
        schema.Type = s2.Type;
        schema.Enum = s2.Enum;
        schema.Default = s2.Default;
        schema.Maximum = s2.Maximum;
        schema.Minimum = s2.Minimum;
        schema.Reference = s2.Reference;
        schema.Pattern = s2.Pattern;
        schema.Nullable = s2.Nullable;
        schema.Required = s2.Required;
        schema.Not = s2.Not;
    }
}
