using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Hosting.Conventions;
using Rocket.Surgery.LaunchPad.Serilog;
using Serilog;

namespace Rocket.Surgery.LaunchPad.Hosting.Conventions;

/// <summary>
///     EnvironmentLoggingConvention.
///     Implements the <see cref="ISerilogConvention" />
/// </summary>
/// <seealso cref="ISerilogConvention" />
[PublicAPI]
[ExportConvention]
public class HealthChecksConvention : IServiceConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services.AddHealthChecks();
    }
}
