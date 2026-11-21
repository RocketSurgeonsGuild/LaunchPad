using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Rocket.Surgery.LaunchPad.Authorization.Joints;

/// <summary>
///     Registers what the <see cref="AuthorizationBehavior" /> shares with the edge: the <see cref="AuthorizationOutcomes" /> of each call and
///     the <see cref="RequestAuthorizationOptions" />.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Application)]
public class ZeroAllocAuthorizationJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        services.AddOptions<RequestAuthorizationOptions>();
        services.TryAddScoped<AuthorizationOutcomes>();
    }
}
