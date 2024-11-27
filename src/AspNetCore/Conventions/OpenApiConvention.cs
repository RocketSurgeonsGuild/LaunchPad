using System.Text;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
[AfterConvention(typeof(AspNetCoreConvention))]
[ConventionCategory(ConventionCategory.Application)]
public partial class OpenApiConvention : IServiceConvention
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
        ArgumentNullException.ThrowIfNull(context);

        services.AddOpenApi(
            options =>
            {
                options.AddSchemaTransformer<RestfulApiActionModelConvention>();
                options.AddSchemaTransformer<ProblemDetailsSchemaFilter>();
                options.AddSchemaTransformer<StronglyTypedIdSchemaFilter>();
                options.AddOperationTransformer<OperationIdFilter>();
                options.AddOperationTransformer<StatusCode201Filter>();
                options.AddOperationTransformer<OperationMediaTypesFilter>();
                options.AddOperationTransformer<AuthorizeFilter>();

            });
        services.AddFluentValidationOpenApi();
    }
}
