using Microsoft.Extensions.Configuration;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Serilog;

using Serilog;

namespace Rocket.Surgery.LaunchPad.Hosting.Conventions;

/// <summary>
///     Defines serilog telemetry configuration
/// </summary>
[PublicAPI]
[ExportConvention]
[ConventionCategory(ConventionCategory.Core)]
public partial class SerilogTelemetryConvention : ISerilogConvention
{
    /// <inheritdoc />
    [RequiresUnreferencedCode("Serilog")]
    public void Register(IConventionContext context, IConfiguration configuration, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
        loggerConfiguration.WriteTo.OpenTelemetry(_ => { }, configuration.GetValue<string>);
}
