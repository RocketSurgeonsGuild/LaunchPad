using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;

[Experimental(Constants.ExperimentalId)]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFluentValidationOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddFluentValidationAutoValidation();
        services.Configure<OpenApiOptions>(
            "v1",
            options => { options.AddSchemaTransformer<FluentValidationOpenApiSchemaTransformer>(); }
        );
        services.TryAddEnumerable(ServiceDescriptor.Transient<IPropertyRuleHandler, RequiredPropertyRuleHandler>());
        services.TryAddEnumerable(ServiceDescriptor.Transient<IPropertyRuleHandler, NotEmptyPropertyRule>());
        services.TryAddEnumerable(ServiceDescriptor.Transient<IPropertyRuleHandler, LengthPropertyRule>());
        services.TryAddEnumerable(ServiceDescriptor.Transient<IPropertyRuleHandler, RegularExpressionPropertyRule>());
        services.TryAddEnumerable(ServiceDescriptor.Transient<IPropertyRuleHandler, ComparisonPropertyRule>());
        services.TryAddEnumerable(ServiceDescriptor.Transient<IPropertyRuleHandler, BetweenPropertyRule>());
        services.TryAddEnumerable(ServiceDescriptor.Transient<IPropertyRuleHandler, EmailPropertyRule>());
        return services;
    }
}