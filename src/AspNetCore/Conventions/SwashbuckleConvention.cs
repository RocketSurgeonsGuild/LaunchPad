using FluentValidation.Validators;
using JetBrains.Annotations;
using MicroElements.Swashbuckle.FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.Conventions;
using Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;
using Rocket.Surgery.LaunchPad.Foundation.Validation;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

[assembly: Convention(typeof(SwashbuckleConvention))]

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions
{
    /// <summary>
    /// ValidationConvention.
    /// Implements the <see cref="IServiceConvention" />
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    /// <seealso cref="IServiceConvention" />
    [PublicAPI]
    [AfterConvention(typeof(AspNetCoreConvention))]
    public class SwashbuckleConvention : IServiceConvention
    {
        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            services.ConfigureOptions<SwashbuckleAddAllDocumentEndpoints>();

            services.AddSwaggerGen(
                options =>
                {
                    options.ConfigureForNodaTime();
                    options.SchemaFilter<ProblemDetailsSchemaFilter>();
                    options.OperationFilter<OperationIdFilter>();
                    options.OperationFilter<StatusCode201Filter>();
                    options.OperationFilter<OperationMediaTypesFilter>();
                    options.OperationFilter<AuthorizeFilter>();
                    options.AddFluentValidationRules();

                    options.MapType<JsonElement>(
                        () => new OpenApiSchema()
                        {
                            Type = "object",
                            AdditionalPropertiesAllowed = true,
                        }
                    );
                    options.MapType<JsonElement?>(
                        () => new OpenApiSchema()
                        {
                            Type = "object",
                            AdditionalPropertiesAllowed = true,
                            Nullable = true,
                        }
                    );

                    options.DocInclusionPredicate(
                        (docName, apiDesc) =>
                        {
                            if (!apiDesc.TryGetMethodInfo(out var methodInfo))
                                return false;
                            return methodInfo.DeclaringType?.GetCustomAttributes(true).OfType<ApiControllerAttribute>()
                                   .Any() ==
                                true;
                        }
                    );

                    options.CustomSchemaIds(
                        type =>
                        {
                            if (type == typeof(global::FluentValidation.Severity))
                                return $"Validation{nameof(global::FluentValidation.Severity)}";
                            return type.IsNested ? type.DeclaringType?.Name + type.Name : type.Name;
                        }
                    );

                    foreach (var item in Directory.EnumerateFiles(AppContext.BaseDirectory, "*.xml")
                       .Where(x => File.Exists(Path.ChangeExtension(x, "dll"))))
                    {
                        try
                        {
                            options.IncludeXmlComments(item);
                        }
                        catch (Exception e)
                        {
                            context.Logger.LogDebug(e, "Error adding XML comments from {XmlFile}", item);
                        }
                    }
                }
            );

            AddFluentValidationRules(services);
        }

        private static void AddFluentValidationRules(IServiceCollection services)
        {
            services.AddSingleton(
                new FluentValidationRule("NotEmpty")
                {
                    Matches = propertyValidator => propertyValidator is INotEmptyValidator,
                    Apply = context =>
                    {
                        var propertyType = context.SchemaFilterContext.Type
                           .GetProperties()
                           .Where(x => x.Name.Equals(context.PropertyKey, StringComparison.OrdinalIgnoreCase))
                           .Select(x => x.PropertyType)
                           .Concat(
                                context.SchemaFilterContext.Type
                                   .GetFields()
                                   .Where(x => x.Name.Equals(context.PropertyKey, StringComparison.OrdinalIgnoreCase))
                                   .Select(x => x.FieldType)
                            )
                           .FirstOrDefault();
                        if (propertyType == typeof(string))
                        {
                            context.Schema.Properties[context.PropertyKey].MinLength = 1;
                        }
                    }
                }
            );

            services.AddSingleton(
                new FluentValidationRule("ValueTypeOrEnum")
                {
                    Matches = propertyValidator => true,
                    Apply = context =>
                    {
                        var propertyType = context.SchemaFilterContext.Type
                           .GetProperties()
                           .Where(x => x.Name.Equals(context.PropertyKey, StringComparison.OrdinalIgnoreCase))
                           .Select(x => x.PropertyType)
                           .Concat(
                                context.SchemaFilterContext.Type
                                   .GetFields()
                                   .Where(x => x.Name.Equals(context.PropertyKey, StringComparison.OrdinalIgnoreCase))
                                   .Select(x => x.FieldType)
                            )
                           .FirstOrDefault();
                        if (propertyType != null &&
                            ( propertyType.IsValueType && Nullable.GetUnderlyingType(propertyType) == null ||
                                propertyType.IsEnum ))
                        {
                            context.Schema.Required.Add(context.PropertyKey);
                            context.Schema.Properties[context.PropertyKey].Nullable = false;
                        }
                    }
                }
            );

            services.AddSingleton(
                new FluentValidationRule("Nullable")
                {
                    Matches = propertyValidator => propertyValidator is INotNullValidator ||
                        propertyValidator is INotEmptyValidator ||
                        propertyValidator is INullValidator,
                    Apply = context =>
                    {
                        context.Schema.Properties[context.PropertyKey].Nullable =
                            !( context.PropertyValidator is INotNullValidator ||
                                context.PropertyValidator is INotEmptyValidator );
                    }
                }
            );

            services.AddSingleton(
                new FluentValidationRule("IsOneOf")
                {
                    Matches = propertyValidator => propertyValidator is StringInValidator,
                    Apply = context =>
                    {
                        var validator = context.PropertyValidator as StringInValidator;
                        context.Schema.Properties[context.PropertyKey].Enum =
                            validator!.Values.Select(x => new OpenApiString(x)).Cast<IOpenApiAny>().ToList();
                    }
                }
            );
        }
    }


}