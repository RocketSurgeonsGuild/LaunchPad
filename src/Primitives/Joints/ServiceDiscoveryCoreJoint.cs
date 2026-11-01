using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Primitives.Joints;

/// <summary>
///     A joint that registers service discovery and adds it to every <see cref="HttpClient" /> the application creates.
/// </summary>
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Application)]
public class ServiceDiscoveryCoreJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddServiceDiscovery();
        services.ConfigureHttpClientDefaults(http => http.AddServiceDiscovery());
    }
}
