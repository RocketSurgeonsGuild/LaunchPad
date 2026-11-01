using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.Primitives;
using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

/// <summary>
///     Applies the existing System.Text.Json options to the MVC and the HTTP JSON options.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[SuppressMessage(
    "Trimming",
    "IL2026:Members annotated with \'RequiresUnreferencedCodeAttribute\' require dynamic access otherwise can break functionality when trimming application code",
    Justification = "Mvc pieces are not specifically added, and problem writers are used by other apis"
)]
[AfterJoint<AspNetCoreJoint>]
[DovetailCategory(DovetailCategory.Application)]
public class SystemJsonTextJoint : IServiceJoint
{
    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="services">The service collection.</param>
    public void Register(IDovetailContext context, IServiceCollection services)
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
