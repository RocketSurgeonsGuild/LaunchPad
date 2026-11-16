using System.Reflection;
using System.Text.Json.Serialization.Metadata;

using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

using Rocket.Surgery.LaunchPad.Foundation;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

/// <summary>
///     Describes each <see cref="Assigned{T}" /> property (a patch request's) as the bare <c>T</c> it is on the wire
///     (<see cref="AssignedJsonConverter{T}" />): optional, since leaving it out leaves the value alone, and nullable when <c>T</c> can be
///     <see langword="null" /> (sending <see langword="null" /> clears the value) or is a value type (<see langword="null" /> leaves it alone,
///     which is what a client generator sends for an optional value it makes nullable).
/// </summary>
internal sealed class AssignedSchemaFilter : IOpenApiSchemaTransformer
{
    public async Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (context.JsonTypeInfo.Kind != JsonTypeInfoKind.Object || schema.Properties is not { Count: > 0 } properties) return;
        foreach (var property in context.JsonTypeInfo.Properties)
        {
            if (property.PropertyType is not { IsGenericType: true } type || type.GetGenericTypeDefinition() != typeof(Assigned<>)) continue;
            var valueType = type.GenericTypeArguments[0];
            var value = await context.GetOrCreateSchemaAsync(valueType, null, cancellationToken).ConfigureAwait(false);
            var description = properties.TryGetValue(property.Name, out var existing) ? existing.Description : null;
            properties[property.Name] = IsNullable(property, valueType) ? Nullable(value) : value;
            if (description is { } && properties[property.Name] is OpenApiSchema described) described.Description ??= description;
            schema.Required?.Remove(property.Name);
        }
    }

    // null is accepted: a value type (Nullable<T> clears, any other leaves the value alone), or a reference type annotated nullable
    // (Assigned<string?>); a null for a reference type that isn't is assigned, and fails the request's validation
    private static bool IsNullable(JsonPropertyInfo property, Type valueType) =>
        valueType.IsValueType
     || ( property.AttributeProvider is PropertyInfo info
         && new NullabilityInfoContext().Create(info).GenericTypeArguments is [{ WriteState: NullabilityState.Nullable, }] );

    private static IOpenApiSchema Nullable(IOpenApiSchema value)
    {
        // a schema that becomes a component (it has a schema id) is replaced with its reference later, so null goes beside it
        if (value is OpenApiSchema { Type: { } type } inline && inline.Metadata?.ContainsKey("x-schema-id") is not true)
        {
            inline.Type = type | JsonSchemaType.Null;
            return inline;
        }

        return new OpenApiSchema { OneOf = [new OpenApiSchema { Type = JsonSchemaType.Null }, value] };
    }
}
