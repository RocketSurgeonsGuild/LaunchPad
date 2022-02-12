using System.Reflection;
using System.Text.Json;
using FluentValidation;
using FluentValidation.Validators;
using MicroElements.Swashbuckle.FluentValidation;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
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
using Rocket.Surgery.LaunchPad.Foundation.Validation;
using Swashbuckle.AspNetCore.SwaggerGen;

[assembly: Convention(typeof(SwashbuckleConvention))]

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions;

/// <summary>
///     ValidationConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[AfterConvention(typeof(AspNetCoreConvention))]
public partial class SwashbuckleConvention : IServiceConvention
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Debug,
        Message = "Error adding XML comments from {XmlFile}"
    )]
    internal static partial void ErrorAddingXMLComments(ILogger logger, Exception exception, string xmlFile);

    private static void AddFluentValidationRules(IServiceCollection services)
    {
        services.AddSingleton(
            new FluentValidationRule("NotEmpty")
               .WithCondition(propertyValidator => propertyValidator is INotEmptyValidator)
               .WithApply(
                    context =>
                    {
                        var propertyType = context.ReflectionContext.PropertyInfo?.DeclaringType ?? context.ReflectionContext.ParameterInfo?.ParameterType;
                        if (propertyType == typeof(string))
                        {
                            context.Schema.Properties[context.PropertyKey].MinLength = 1;
                        }
                    }
                )
        );

        services.AddSingleton(
            new FluentValidationRule("ValueTypeOrEnum")
               .WithApply(
                    context =>
                    {
                        var propertyType = context.ReflectionContext.PropertyInfo?.DeclaringType ?? context.ReflectionContext.ParameterInfo?.ParameterType;
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
               .WithApply(
                    context =>
                    {
                        context.Schema.Properties[context.PropertyKey].Nullable =
                            context.PropertyValidator is not (INotNullValidator or INotEmptyValidator)
                         || ( context.ReflectionContext.ParameterInfo is { } pai && getNullableValue(pai.GetNullability(), pai.ParameterType) )
                         || ( context.ReflectionContext.PropertyInfo is PropertyInfo pi && getNullableValue(pi.GetNullability(), pi.PropertyType) )
                         || ( context.ReflectionContext.PropertyInfo is FieldInfo fi && getNullableValue(fi.GetNullability(), fi.FieldType) )
                            ;

                        static bool getNullableValue(Nullability nullability, Type propertyType)
                        {
                            return nullability switch
                            {
                                Nullability.Nullable    => true,
                                Nullability.NonNullable => false,
                                Nullability.NotDefined  => !propertyType.IsValueType || Nullable.GetUnderlyingType(propertyType) is not null,
                                _                       => false
                            };
                        }
                    }
                )
        );

        services.AddSingleton(
            new FluentValidationRule("IsOneOf")
               .WithCondition(propertyValidator => propertyValidator is IStringInValidator)
               .WithApply(
                    context =>
                    {
                        var validator = context.PropertyValidator as IStringInValidator;
                        context.Schema.Properties[context.PropertyKey].Enum =
                            validator!.Values.Select(x => new OpenApiString(x)).Cast<IOpenApiAny>().ToList();
                    }
                )
        );
    }

    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
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

        services.AddFluentValidationRulesToSwagger();
        services.AddSwaggerGen(
            options =>
            {
                options.SchemaFilter<ProblemDetailsSchemaFilter>();
                options.OperationFilter<OperationIdFilter>();
                options.OperationFilter<StatusCode201Filter>();
                options.OperationFilter<OperationMediaTypesFilter>();
                options.OperationFilter<AuthorizeFilter>();

                options.MapType<JsonElement>(
                    () => new OpenApiSchema
                    {
                        Type = "object",
                        AdditionalPropertiesAllowed = true,
                    }
                );
                options.MapType<JsonElement?>(
                    () => new OpenApiSchema
                    {
                        Type = "object",
                        AdditionalPropertiesAllowed = true,
                        Nullable = true,
                    }
                );

                options.DocInclusionPredicate(
                    (_, apiDesc) =>
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
                        if (type == typeof(Severity))
                            return $"Validation{nameof(Severity)}";
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
#pragma warning disable CA1031
                    catch (Exception e)
#pragma warning restore CA1031
                    {
                        ErrorAddingXMLComments(context.Logger, e, item);
                    }
                }
            }
        );

        AddFluentValidationRules(services);
    }
}
