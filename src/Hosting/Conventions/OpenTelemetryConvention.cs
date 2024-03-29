using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Hosting.Telemetry;
using Rocket.Surgery.LaunchPad.Serilog;

namespace Rocket.Surgery.LaunchPad.Hosting.Conventions;

/// <summary>
///     EnvironmentLoggingConvention.
///     Implements the <see cref="ISerilogConvention" />
/// </summary>
/// <seealso cref="ISerilogConvention" />
[PublicAPI]
[ExportConvention]
public class OpenTelemetryConvention : IServiceConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services
           .AddOpenTelemetry()
           .ApplyConventions(context)
            ;
    }
}
