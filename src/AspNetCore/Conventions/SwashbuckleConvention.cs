using FluentValidation.Validators;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Extensions;
using Rocket.Surgery.LaunchPad.AspNetCore.Conventions;
using Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;
using Rocket.Surgery.LaunchPad.AspNetCore.OpenApi.Validation.Core;
using Rocket.Surgery.LaunchPad.Foundation.Validation;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using FluentValidationRule = Rocket.Surgery.LaunchPad.AspNetCore.OpenApi.Validation.FluentValidationRule;

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

            services.AddOptions<SwaggerGenOptions>()
               .Configure<IOptions<JsonOptions>>(
                    (options, mvcOptions) => { options.ConfigureForNodaTime(mvcOptions.Value.JsonSerializerOptions); }
                );
            services.AddSwaggerGen(
                options =>
                {
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
                   .MatchesValidatorWithNoCondition()
                   .MatchesValidator(propertyValidator => propertyValidator is INotEmptyValidator)
                   .WithApply(
                        context =>
                        {
                            var propertyType = context.MemberInfo.GetMemberType();
                            if (propertyType == typeof(string))
                            {
                                context.Schema.Properties[context.PropertyKey].MinLength = 1;
                            }
                        }
                    )
            );

            services.AddSingleton(
                new FluentValidationRule(
                        "ValueTypeOrEnum"
                    )
                   .MatchesValidatorWithNoCondition()
                   .WithApply(
                        context =>
                        {
                            var propertyType = context.MemberInfo.GetMemberType();
                            if (propertyType != null &&
                                ( ( propertyType.IsValueType && Nullable.GetUnderlyingType(propertyType) == null ) ||
                                    propertyType.IsEnum ))
                            {
                                context.Schema.Required.Add(context.PropertyKey);
                                context.Schema.Properties[context.PropertyKey].Nullable = false;
                            }
                        }
                    )
            );

            services.AddSingleton(
                new FluentValidationRule("Nullable")
                   .MatchesValidatorWithNoCondition()
                   .WithApply(
                        context =>
                        {
                            context.Schema.Properties[context.PropertyKey].Nullable =
                                context.PropertyValidator is not (INotNullValidator or INotEmptyValidator)
                             || ( context.MemberInfo is PropertyInfo pi && pi.GetNullability() switch
                                {
                                    Nullability.Nullable    => true,
                                    Nullability.NonNullable => false,
                                    Nullability.NotDefined  => !pi.PropertyType.IsValueType || Nullable.GetUnderlyingType(pi.PropertyType) is not null,
                                    _                       => false
                                } );
                        }
                    )
            );

            services.AddSingleton(
                new FluentValidationRule("IsOneOf")
                   .MatchesValidatorWithNoCondition()
                   .MatchesValidator(propertyValidator => propertyValidator is StringInValidator)
                   .WithApply(
                        context =>
                        {
                            var validator = context.PropertyValidator as StringInValidator;
                            context.Schema.Properties[context.PropertyKey].Enum =
                                validator!.Values.Select(x => new OpenApiString(x)).Cast<IOpenApiAny>().ToList();
                        }
                    )
            );
        }
    }
}