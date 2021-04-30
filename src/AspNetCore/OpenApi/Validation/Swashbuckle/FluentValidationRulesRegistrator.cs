using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using FluentValidationOperationFilter = Rocket.Surgery.LaunchPad.AspNetCore.OpenApi.Validation.Swashbuckle.FluentValidationOperationFilter;
using FluentValidationRules = Rocket.Surgery.LaunchPad.AspNetCore.OpenApi.Validation.Swashbuckle.FluentValidationRules;

// ReSharper disable once CheckNamespace
namespace Swashbuckle.AspNetCore.Swagger
{
    /// <summary>
    /// Registration extensions.
    /// </summary>
    public static class FluentValidationRulesRegistrator
    {
        /// <summary>
        /// Adds fluent validation rules to swagger.
        /// </summary>
        /// <param name="options">Swagger options.</param>
        public static void AddFluentValidationRules(this SwaggerGenOptions options)
        {
            options.SchemaFilter<FluentValidationRules>();
            options.OperationFilter<FluentValidationOperationFilter>();
        }
    }
}