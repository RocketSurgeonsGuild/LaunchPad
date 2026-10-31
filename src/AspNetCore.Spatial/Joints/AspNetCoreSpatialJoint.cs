using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

/// <summary>
///     Joint to register spatial types
/// </summary>
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Application)]
public class AspNetCoreSpatialJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        // There is nothing to register yet: the OpenAPI mapping of NetTopologySuite types that used to be sketched here is not wired up.
    }
}
