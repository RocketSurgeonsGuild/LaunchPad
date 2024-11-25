using System.Numerics;
using System.Text.Json.Serialization.Metadata;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Internal;
using FluentValidation.Validators;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;

namespace Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;

internal static class Constants
{
    public const string ExperimentalId = "RSGEXP";
}

[Experimental(Constants.ExperimentalId)]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFluentValidationOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddFluentValidationAutoValidation();
        services.Configure<OpenApiOptions>(
            options =>
            {
options.AddSchemaTransformer<FluentValidationOpenApiSchemaTransformer>();
            }
        );
                services.TryAddEnumerable(ServiceDescriptor.Transient<IPropertyRuleHandler, RequiredPropertyRule>());
                services.TryAddEnumerable(ServiceDescriptor.Transient<IPropertyRuleHandler, NotEmptyPropertyRule>());
                services.TryAddEnumerable(ServiceDescriptor.Transient<IPropertyRuleHandler, LengthPropertyRule>());
                services.TryAddEnumerable(ServiceDescriptor.Transient<IPropertyRuleHandler, RegularExpressionPropertyRule>());
                services.TryAddEnumerable(ServiceDescriptor.Transient<IPropertyRuleHandler, ComparisonPropertyRule>());
                services.TryAddEnumerable(ServiceDescriptor.Transient<IPropertyRuleHandler, BetweenPropertyRule>());
        return services;
    }
}

[Experimental(Constants.ExperimentalId)]
public record OpenApiValidationContext
(
    OpenApiSchema TypeSchema,
    OpenApiSchema PropertySchema,
    OpenApiSchemaTransformerContext TransformerContext,
    IPropertyValidator PropertyValidator,
    JsonPropertyInfo PropertyInfo,
    IRuleComponent RuleComponent);

[Experimental(Constants.ExperimentalId)]
public interface IPropertyRuleHandler
{
    Task HandleAsync(OpenApiValidationContext context, CancellationToken cancellationToken);
}

[Experimental(Constants.ExperimentalId)]
public class FluentValidationOpenApiSchemaTransformer(IEnumerable<IPropertyRuleHandler> ruleDefinitionHandlers) : IOpenApiSchemaTransformer
{
    public async Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (context.JsonPropertyInfo is not { } propertyInfo) return;
        var validatorType = typeof(IValidator<>).MakeGenericType(context.JsonTypeInfo.Type);
        if (context.ApplicationServices.GetService(validatorType) is not IValidator validator) return;

        var descriptor = validator.CreateDescriptor();
        foreach (var member in descriptor.GetMembersWithValidators())
        {
            foreach (var (propertyValidator, component) in member)
            {
                if (!schema.Properties.TryGetValue(member.Key, out var property)) continue;

                foreach (var item in ruleDefinitionHandlers)
                {
                    var ctx = new OpenApiValidationContext(
                        schema,
                        property,
                        context,
                        propertyValidator,
                        propertyInfo,
                        component
                    );
                    await item.HandleAsync(ctx, cancellationToken);
                }
            }
        }
    }
}

[Experimental(Constants.ExperimentalId)]
public sealed class RequiredPropertyRule : IPropertyRuleHandler
{
    Task IPropertyRuleHandler.HandleAsync(OpenApiValidationContext context, CancellationToken cancellationToken)
    {
        if (context.PropertyValidator is not INotNullValidator or INotEmptyValidator) return Task.CompletedTask;

        context.TypeSchema.Required.Add(context.PropertyInfo.Name);
        return Task.CompletedTask;
    }
}

[Experimental(Constants.ExperimentalId)]
public sealed class NotEmptyPropertyRule : IPropertyRuleHandler
{
    Task IPropertyRuleHandler.HandleAsync(OpenApiValidationContext context, CancellationToken cancellationToken)
    {
        if (context is { PropertyValidator: not INotEmptyValidator } or { PropertySchema.MinLength: > 1 } or { PropertySchema.Type: not ("string" or "array") })
            return Task.CompletedTask;
        context.PropertySchema.MinLength = 1;
        return Task.CompletedTask;
    }
}

[Experimental(Constants.ExperimentalId)]
public sealed class LengthPropertyRule : IPropertyRuleHandler
{
    Task IPropertyRuleHandler.HandleAsync(OpenApiValidationContext context, CancellationToken cancellationToken)
    {
        if (context.PropertyValidator is not ILengthValidator validator) return Task.CompletedTask;

        if (context.PropertySchema.Type == "array")
        {
            if (validator.Max > 0)
                context.PropertySchema.MaxItems = validator.Max;

            if (validator.Min > 0)
                context.PropertySchema.MinItems = validator.Min;
        }
        else
        {
            if (validator.Max > 0)
                context.PropertySchema.MaxLength = validator.Max;
            if (validator.Min > 0)
                context.PropertySchema.MinLength = validator.Min;
        }

        return Task.CompletedTask;
    }
}

[Experimental(Constants.ExperimentalId)]
public sealed class RegularExpressionPropertyRule : IPropertyRuleHandler
{
    Task IPropertyRuleHandler.HandleAsync(OpenApiValidationContext context, CancellationToken cancellationToken)
    {
        if (context is not { PropertyValidator: IRegularExpressionValidator validator }) return Task.CompletedTask;

        var anyPatterns = context.PropertySchema.AllOf.Any(schema => schema.Pattern is { });
        if (context.PropertySchema is { Pattern: { } } || anyPatterns)
        {
            if (!anyPatterns) context.PropertySchema.AllOf.Add(new() { Pattern = context.PropertySchema.Pattern });
            context.PropertySchema.AllOf.Add(new() { Pattern = validator.Expression });
            context.PropertySchema.Pattern = null;
        }
        else
        {
            context.PropertySchema.Pattern = validator.Expression;
        }

        return Task.CompletedTask;
    }
}

[Experimental(Constants.ExperimentalId)]
public sealed class EmailPropertyRule : IPropertyRuleHandler
{
    Task IPropertyRuleHandler.HandleAsync(OpenApiValidationContext context, CancellationToken cancellationToken)
    {
        if (context is not { PropertyValidator: IEmailValidator validator }) return Task.CompletedTask;
        context.PropertySchema.Format = "email";
        return Task.CompletedTask;
    }
}

[Experimental(Constants.ExperimentalId)]
public sealed class ComparisonPropertyRule : IPropertyRuleHandler
{
    Task IPropertyRuleHandler.HandleAsync(OpenApiValidationContext context, CancellationToken cancellationToken)
    {
        if (context is not { PropertyValidator: IComparisonValidator validator }) return Task.CompletedTask;

        if (!validator.ValueToCompare.IsNumeric()) return Task.CompletedTask;
        var valueToCompare = Convert.ToDecimal(validator.ValueToCompare);
        var schemaProperty = context.PropertySchema;

        switch (validator)
        {
            case { Comparison: Comparison.GreaterThanOrEqual }:
                {
                    schemaProperty.Minimum = valueToCompare;
                    return Task.CompletedTask;
                }

            case { Comparison: Comparison.GreaterThan }:
                {
                    schemaProperty.Minimum = valueToCompare;
                    schemaProperty.ExclusiveMinimum = true;
                    return Task.CompletedTask;
                }

            case { Comparison: Comparison.LessThanOrEqual }:
                {
                    schemaProperty.Maximum = valueToCompare;
                    return Task.CompletedTask;
                }

            case { Comparison: Comparison.LessThan }:
                {
                    schemaProperty.Maximum = valueToCompare;
                    schemaProperty.ExclusiveMaximum = true;
                    return Task.CompletedTask;
                }
        }

        return Task.CompletedTask;
    }
}

internal static class Extensions
{
    internal static bool IsNumeric(this object value) => value is int || value is long || value is float || value is double || value is decimal;
}

[Experimental(Constants.ExperimentalId)]
public sealed class BetweenPropertyRule : IPropertyRuleHandler
{
    Task IPropertyRuleHandler.HandleAsync(OpenApiValidationContext context, CancellationToken cancellationToken)
    {
        if (context is not { PropertyValidator: IBetweenValidator validator }) return Task.CompletedTask;

        var schemaProperty = context.PropertySchema;
        if (validator.From.IsNumeric())
        {
            schemaProperty.Minimum = Convert.ToDecimal(validator.From);
            if (validator.Name == "ExclusiveBetweenValidator") schemaProperty.ExclusiveMinimum = true;
        }

        if (validator.To.IsNumeric())
        {
            schemaProperty.Maximum = Convert.ToDecimal(validator.To);
            if (validator.Name == "ExclusiveBetweenValidator") schemaProperty.ExclusiveMaximum = true;
        }

        return Task.CompletedTask;
    }
}
