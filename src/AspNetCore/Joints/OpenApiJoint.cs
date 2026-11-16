using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
                                // An operation's nested types (CreateRocket.Request) are named after their container: by their own name, every
                                // nested Request would be one Request schema, and a client generated from the document would send the wrong body.
                                options.CreateSchemaReferenceId = static type => type.Type is { DeclaringType: { } container, IsGenericType: false, }
                                    ? container.Name + type.Type.Name
                                    : OpenApiOptions.CreateDefaultSchemaReferenceId(type);
                                options.AddSchemaTransformer<AssignedSchemaFilter>();
                                options.AddSchemaTransformer<ProblemDetailsSchemaFilter>();
                                options.AddOperationTransformer<OperationIdFilter>();
                                options.AddOperationTransformer<StatusCode201Filter>();
                                options.AddOperationTransformer<OperationMediaTypesFilter>();
                                options.AddOperationTransformer<AuthorizeFilter>();
                            }
        );
    }

    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Debug,
        Message = "Error adding XML comments from {XmlFile}"
    )]
    internal static partial void ErrorAddingXMLComments(ILogger logger, Exception exception, string xmlFile);
}

