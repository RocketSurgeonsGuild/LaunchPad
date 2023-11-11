using System.Text;
using System.Text.Json;
using FluentValidation;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.Composition;
using Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions;

/// <summary>
///     ValidationConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
[AfterConvention(typeof(AspNetCoreConvention))]
public partial class SwashbuckleConvention : IServiceConvention
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Debug,
        Message = "Error adding XML comments from {XmlFile}"
    )]
    internal static partial void ErrorAddingXMLComments(ILogger logger, Exception exception, string xmlFile);

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

        services.AddFluentValidationRulesToSwagger(
            options => options.SetNotNullableIfMinLengthGreaterThenZero = true,
            options => options.ServiceLifetime = ServiceLifetime.Singleton
        );

//        services.TryAddEnumerable(ServiceDescriptor.Transient<IApiDescriptionProvider, StronglyTypedIdApiDescriptionProvider>());

        services.AddOptions<SwaggerGenOptions>()
                .Configure<IOptions<JsonOptions>>(
                     (options, mvcOptions) => options.ConfigureForNodaTime(mvcOptions.Value.JsonSerializerOptions)
                 );
        services.AddSwaggerGen(
            options =>
            {
                options.SchemaFilter<RestfulApiActionModelConvention>();
                options.SchemaFilter<ProblemDetailsSchemaFilter>();
                options.SchemaFilter<StronglyTypedIdSchemaFilter>();
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
                        return methodInfo.DeclaringType?.GetCustomAttributes(true).OfType<ApiControllerAttribute>().Any() == true;
                    }
                );

                string nestedTypeName(Type type)
                {
                    // ReSharper disable once NullableWarningSuppressionIsUsed
                    return type.IsNested ? schemaIdSelector(type.DeclaringType!) + type.Name : type.Name;
                }

                string schemaIdSelector(Type type)
                {
                    if (type == typeof(Severity)) return $"Validation{nameof(Severity)}";
                    if (type.IsGenericType)
                    {
                        var sb = new StringBuilder();
                        var name = nestedTypeName(type);
                        name = name[..name.IndexOf('`', StringComparison.Ordinal)];
                        sb.Append(name);
                        foreach (var gt in type.GetGenericArguments())
                            sb.Append('_').Append(schemaIdSelector(gt));

                        return sb.ToString();
                    }

                    return nestedTypeName(type);
                }

                options.CustomSchemaIds(schemaIdSelector);

                foreach (var item in Directory.EnumerateFiles(AppContext.BaseDirectory, "*.xml")
                                              .Where(x => File.Exists(Path.ChangeExtension(x, "dll"))))
                {
                    try
                    {
                        options.IncludeXmlComments(item, true);
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
    }
}
