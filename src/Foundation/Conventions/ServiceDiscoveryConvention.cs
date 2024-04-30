using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Foundation.Conventions;

/// <summary>
///     Service conventions using service discovery
/// </summary>
[PublicAPI]
[ExportConvention]
public class ServiceDiscoveryConvention : IServiceConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services.AddServiceDiscovery();

        services.ConfigureHttpClientDefaults(
            http =>
            {
                http.AddStandardResilienceHandler();
                http.AddServiceDiscovery();
            }
        );
    }
}