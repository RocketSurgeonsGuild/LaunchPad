using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Foundation;
using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions;

/// <summary>
///     ValidationConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
/// <seealso cref="IServiceConvention" />
/// <remarks>
///     Create a new SystemJsonTextConvention
/// </remarks>
/// <param name="options"></param>
[PublicAPI]
[ExportConvention]
[AfterConvention<AspNetCoreConvention>]
[ConventionCategory(ConventionCategory.Application)]
public class SystemJsonTextConvention(FoundationOptions? options = null) : IServiceConvention
{
    private readonly FoundationOptions _options = options ?? new FoundationOptions();

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
           .Configure<IServiceProvider>((options, provider) => ExistingValueOptions.Apply(provider, options.JsonSerializerOptions, Options.DefaultName));
        services
           .AddOptions<HttpJsonOptions>()
           .Configure<IServiceProvider>((options, provider) => ExistingValueOptions.Apply(provider, options.SerializerOptions, Options.DefaultName));
    }
}
