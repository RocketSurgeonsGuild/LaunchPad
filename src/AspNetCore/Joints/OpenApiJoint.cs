using System.Text.Json.Nodes;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using Rocket.Surgery.LaunchPad.AspNetCore.Composition;
using Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;
using Rocket.Surgery.LaunchPad.AspNetCore.OpenApi;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

/// <summary>
///     Registers OpenAPI document generation, with the LaunchPad schema and operation transformers.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[AfterJoint<AspNetCoreJoint>]
[DovetailCategory(DovetailCategory.Application)]
public partial class OpenApiJoint : IServiceJoint
{
    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="services">The service collection.</param>
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(context);

        services.AddOpenApi(options =>
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
        schema.AddExtension("x-schema-id", new JsonNodeExtension($"{context.JsonTypeInfo.Type.DeclaringType.Name}{context.JsonTypeInfo.Type.Name}"));
        return Task.CompletedTask;
    }
}
