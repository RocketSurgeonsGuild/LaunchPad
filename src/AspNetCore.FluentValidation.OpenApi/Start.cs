using FluentValidation;
using FluentValidation.AspNetCore;
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
public class ServiceCollectionExtensions
{
    public static IServiceCollection AddFluentValidationOpenApi(this IServiceCollection services)
    {
        services.AddFluentValidation();
        services.TryAddSingleton<IOpenApiSchemaTransformer, FluentValidationOpenApiSchemaTransformer>();
        return services;
    }
}

[Experimental(Constants.ExperimentalId)]
public class FluentValidationOpenApiSchemaTransformer : IOpenApiSchemaTransformer
{
    public FluentValidationOpenApiSchemaTransformer()
    {

    }

    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (!typeof(IValidator).IsAssignableTo(context.JsonTypeInfo.Type) || context.ApplicationServices.GetRequiredService(context.JsonTypeInfo.Type) is not IEnumerable<IValidationRule> rules)
        {
            return Task.CompletedTask;
        }

        foreach (var rule in rules)
        {
            rule.
            if (rule is IValidationRule<OpenApiSchema> schemaRule)
            {

            }
        }
    }
}
