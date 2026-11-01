using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Primitives.Joints;

/// <summary>
///     A joint that adds the standard resilience handler to every <see cref="HttpClient" /> the application creates.
/// </summary>
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Application)]
public class StandardResilienceJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.ConfigureHttpClientDefaults(http => http.AddStandardResilienceHandler());
    }
}
