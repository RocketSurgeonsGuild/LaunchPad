using System.Globalization;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

using Rocket.Surgery.LaunchPad.AspNetCore.Validation;

using ZeroAlloc.Validation;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

/// <summary>
///     Describes the ZeroAlloc.Validation attributes of a type's properties in its OpenAPI schema: required properties, lengths, ranges,
///     patterns and e-mail addresses. It also marks the members of the problem details LaunchPad writes as required: the type and title, and
///     the errors of a validation problem.
/// </summary>
/// <remarks>
///     The attributes are read from the members System.Text.Json serializes, through each property's attribute provider, so the schema
///     property and its rules are matched by the serialized name. Rules that cannot be described (<c>[Must]</c>, custom attributes) are left out.
/// </remarks>
[PublicAPI]
public sealed class ZeroAllocOpenApiSchemaTransformer : IOpenApiSchemaTransformer
{
    /// <inheritdoc />
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentNullException.ThrowIfNull(context);
        // Microsoft.OpenApi leaves the properties of a schema that has none (a scalar, an array) null.
        if (schema.Properties is not { Count: > 0 } properties) return Task.CompletedTask;

        RequireProblemDetailsMembers(schema, context.JsonTypeInfo.Type);

        foreach (var jsonProperty in context.JsonTypeInfo.Properties)
        {
            if (jsonProperty.AttributeProvider is not { } provider) continue;
            if (!properties.TryGetValue(jsonProperty.Name, out var value) || value is not OpenApiSchema property) continue;
            foreach (var attribute in provider.GetCustomAttributes(typeof(ValidationAttribute), true).OfType<ValidationAttribute>())
            {
                Apply(schema, jsonProperty.Name, property, attribute);
            }
        }

        return Task.CompletedTask;
    }

    private static void Apply(OpenApiSchema schema, string name, OpenApiSchema property, ValidationAttribute attribute)
    {
        var isArray = property.Type is { } type && type.HasFlag(JsonSchemaType.Array);
        switch (attribute)
        {
            case global::ZeroAlloc.Validation.NotNullAttribute:
                Require(schema, name);
                break;
            case NotEmptyAttribute:
                Require(schema, name);
                if (isArray) property.MinItems ??= 1;
                else if (property.Type is { } stringType && stringType.HasFlag(JsonSchemaType.String)) property.MinLength ??= 1;
                break;
            case MinLengthAttribute minLength:
                SetMinimumLength(property, isArray, minLength.Min);
                break;
            case MaxLengthAttribute maxLength:
                SetMaximumLength(property, isArray, maxLength.Max);
                break;
            case LengthAttribute length:
                SetMinimumLength(property, isArray, length.Min);
                SetMaximumLength(property, isArray, length.Max);
                break;
            case InclusiveBetweenAttribute between:
                property.Minimum = Number(between.Min);
                property.Maximum = Number(between.Max);
                break;
            case ExclusiveBetweenAttribute between:
                property.ExclusiveMinimum = Number(between.Min);
                property.ExclusiveMaximum = Number(between.Max);
                break;
            case GreaterThanOrEqualToAttribute comparison:
                property.Minimum = Number(comparison.Value);
                break;
            case GreaterThanAttribute comparison:
                property.ExclusiveMinimum = Number(comparison.Value);
                break;
            case LessThanOrEqualToAttribute comparison:
                property.Maximum = Number(comparison.Value);
                break;
            case LessThanAttribute comparison:
                property.ExclusiveMaximum = Number(comparison.Value);
                break;
            case MatchesAttribute matches:
                property.Pattern = matches.Pattern;
                break;
            case EmailAddressAttribute:
                property.Format = "email";
                break;
            default:
                break;
        }
    }

    // The problem details LaunchPad writes always have a type and a title, and the validation ones their errors.
    private static void RequireProblemDetailsMembers(OpenApiSchema schema, Type type)
    {
        if (!typeof(ProblemDetails).IsAssignableFrom(type)) return;
        RequireIfPresent(schema, "type");
        RequireIfPresent(schema, "title");
        if (typeof(HttpValidationProblemDetails).IsAssignableFrom(type) || typeof(ValidationFailureProblemDetails).IsAssignableFrom(type))
            RequireIfPresent(schema, "errors");
    }

    private static void RequireIfPresent(OpenApiSchema schema, string name)
    {
        if (schema.Properties?.ContainsKey(name) == true) Require(schema, name);
    }

    private static void Require(OpenApiSchema schema, string name)
    {
        schema.Required ??= new HashSet<string>(StringComparer.Ordinal);
        schema.Required.Add(name);
    }

    private static void SetMinimumLength(OpenApiSchema property, bool isArray, int value)
    {
        if (value <= 0) return;
        if (isArray) property.MinItems = value;
        else property.MinLength = value;
    }

    private static void SetMaximumLength(OpenApiSchema property, bool isArray, int value)
    {
        if (value is <= 0 or int.MaxValue) return;
        if (isArray) property.MaxItems = value;
        else property.MaxLength = value;
    }

    // OpenAPI 3.1 (JSON Schema): exclusiveMinimum and exclusiveMaximum are numbers, not flags.
    private static string Number(double value) => value.ToString("R", CultureInfo.InvariantCulture);
}
