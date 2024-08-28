using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Foundation;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;
using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

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
public class SystemJsonTextConvention : IServiceConvention
{
    private readonly FoundationOptions _options;

    /// <summary>
    ///     Create a new SystemJsonTextConvention
    /// </summary>
    /// <param name="options"></param>
    public SystemJsonTextConvention(FoundationOptions? options = null)
    {
        _options = options ?? new FoundationOptions();
    }

    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(context);

        services
           .AddOptions<MvcJsonOptions>()
           .Configure<IServiceProvider>(
                (options, provider) => ActivatorUtilities
                                      .CreateInstance<ExistingValueOptionsFactory<JsonSerializerOptions>>(provider, options.JsonSerializerOptions)
                                      .Create(nameof(MvcJsonOptions))
            );
        services
           .AddOptions<HttpJsonOptions>()
           .Configure<IServiceProvider>(
                (options, provider) => ActivatorUtilities
                                      .CreateInstance<ExistingValueOptionsFactory<JsonSerializerOptions>>(provider, options.SerializerOptions)
                                      .Create(nameof(HttpJsonOptions))
            );
    }
}