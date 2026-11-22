using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Authorization.Joints;

/// <summary>
///     Registers what the <see cref="AuthorizationBehavior" /> shares with the edge: the <see cref="AuthorizationOutcomes" /> of each call.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Application)]
public class ZeroAllocAuthorizationJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services) => services.AddLaunchPadAuthorization();
}
