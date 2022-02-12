using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Foundation;

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
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        services
           .AddOptions<JsonOptions>()
           .Configure<IServiceProvider>(
                (options, provider) => ActivatorUtilities
                                      .CreateInstance<ExistingValueOptionsFactory<JsonSerializerOptions>>(provider, options.JsonSerializerOptions)
                                      .Create(nameof(JsonOptions))
            );
    }
}
