using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.LaunchPad.Serilog;
using Serilog;

namespace Rocket.Surgery.LaunchPad.Foundation.Conventions;

/// <summary>
/// Defines serilog telemetry configuration
/// </summary>
[PublicAPI]
[ExportConvention]
[ConventionCategory(ConventionCategory.Core)]
public partial class SerilogTelemetryConvention : ISerilogConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceProvider services, LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration.WriteTo.OpenTelemetry(_ => { }, configuration.GetValue<string>);
    }
}
