using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Rocket.Surgery.LaunchPad.AspNetCore.HealthChecks;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

/// <summary>
///     Registers the health check services and the options of LaunchPad's health check endpoints (<c>MapLaunchPadHealthChecks</c>), bound from
///     the <see cref="LaunchPadHealthCheckOptions.SectionName" /> configuration section.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <remarks>
///     When <see cref="LaunchPadHealthCheckOptions.Port" /> is set, Kestrel also listens on that port, next to the application's own addresses,
///     and the port serves the health check endpoints alone: anything else on it gets a 404, and so do the health check endpoints on any other
///     port. When the server doesn't listen on the port after all, the health check endpoints answer on the application's own addresses.
/// </remarks>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Application)]
public class HealthChecksJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        services.AddHealthChecks();
        // Optionary binds LaunchPadHealthCheckOptions from its section without reflection, and describes it in appsettings.schema.json.
        services.AddOptionaryConfigurationOptions();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<KestrelServerOptions>, HealthCheckKestrelSetup>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IStartupFilter, HealthCheckPortStartupFilter>());
    }
}
