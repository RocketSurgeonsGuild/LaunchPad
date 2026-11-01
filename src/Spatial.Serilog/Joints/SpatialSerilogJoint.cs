using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;

using Serilog;

namespace Rocket.Surgery.LaunchPad.Spatial.Joints;

/// <summary>
///     A joint that destructures the NetTopologySuite types in the logger Dovetail creates, the way
///     <see cref="NetTopologySuiteLoggerConfigurationExtensions.NetTopologySuiteTypes" /> does.
///     Implements the <see cref="ISerilogJoint" />
/// </summary>
/// <seealso cref="ISerilogJoint" />
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Core)]
public class SpatialSerilogJoint : ISerilogJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services, LoggerConfiguration loggerConfiguration)
    {
        ArgumentNullException.ThrowIfNull(loggerConfiguration);

        loggerConfiguration.Destructure.NetTopologySuiteTypes();
    }
}
