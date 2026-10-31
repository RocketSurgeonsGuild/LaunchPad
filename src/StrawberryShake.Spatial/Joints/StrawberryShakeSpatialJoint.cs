using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;
using Microsoft.Extensions.DependencyInjection;
using StrawberryShake.Serialization;

namespace Rocket.Surgery.LaunchPad.StrawberryShake.Spatial.Joints;

/// <summary>
///     Adds the spatial serializers to StrawberryShake
/// </summary>
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Core)]
public class StrawberryShakeSpatialJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        services.AddSingleton<ISerializer, GeometrySerializer>();
        // services.AddSerializer<CoordinatesSerializer>();
    }
}
