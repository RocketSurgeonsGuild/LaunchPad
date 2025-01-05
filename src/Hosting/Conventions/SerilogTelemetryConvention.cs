using Microsoft.Extensions.Configuration;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Serilog;

using Serilog;

namespace Rocket.Surgery.LaunchPad.Hosting.Conventions;

/// <summary>
///     Defines serilog telemetry configuration
/// </summary>
[PublicAPI]
[ConventionCategory(ConventionCategory.Core)]
[UnconditionalSuppressMessage(
    "Trimming",
    "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
    Justification = "Getting string values"
)]
public partial class SerilogTelemetryConvention : ISerilogConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
        loggerConfiguration.WriteTo.OpenTelemetry(_ => { }, configuration.GetValue<string>);
}
