using System;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.Conventions;
using Rocket.Surgery.LaunchPad.Foundation;

[assembly: Convention(typeof(SystemJsonTextConvention))]

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions;

/// <summary>
///     ValidationConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
/// <seealso cref="IServiceConvention" />
[PublicAPI]
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
        _options = options ?? new();
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
