using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.Composition;
using Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;
using Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions;

/// <summary>
///     ValidationConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
[AfterConvention<AspNetCoreConvention>]
[ConventionCategory(ConventionCategory.Application)]
public partial class OpenApiConvention : IServiceConvention
{
    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(context);

        services.AddOpenApi(
            options =>
            {
                options.AddSchemaTransformer<NestedTypeSchemaFilter>();
                options.AddSchemaTransformer<RestfulApiActionModelConvention>();
                options.AddSchemaTransformer<ProblemDetailsSchemaFilter>();
                options.AddSchemaTransformer<StronglyTypedIdSchemaFilter>();
                options.AddOperationTransformer<OperationIdFilter>();
                options.AddOperationTransformer<StatusCode201Filter>();
                options.AddOperationTransformer<OperationMediaTypesFilter>();
                options.AddOperationTransformer<AuthorizeFilter>();
            }
        );
        services.AddFluentValidationOpenApi();
    }

    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Debug,
        Message = "Error adding XML comments from {XmlFile}"
    )]
    internal static partial void ErrorAddingXMLComments(ILogger logger, Exception exception, string xmlFile);
}

internal class NestedTypeSchemaFilter : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (context is not { JsonTypeInfo.Type.DeclaringType: { } }) return Task.CompletedTask;
        schema.Annotations["x-schema-id"] = $"{context.JsonTypeInfo.Type.DeclaringType.Name}{context.JsonTypeInfo.Type.Name}";
        return Task.CompletedTask;
    }
}
