using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;

using Serilog;

namespace Rocket.Surgery.LaunchPad.Foundation.Joints;

/// <summary>
///     A joint that destructures the Newtonsoft.Json types in the logger Dovetail creates, the way
///     <see cref="NewtonsoftJsonLoggerConfigurationExtensions.NewtonsoftJsonTypes" /> does.
///     Implements the <see cref="ISerilogJoint" />
/// </summary>
/// <seealso cref="ISerilogJoint" />
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Core)]
public class NewtonsoftJsonSerilogJoint : ISerilogJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services, LoggerConfiguration loggerConfiguration)
    {
        ArgumentNullException.ThrowIfNull(loggerConfiguration);

        loggerConfiguration.Destructure.NewtonsoftJsonTypes();
    }
}
