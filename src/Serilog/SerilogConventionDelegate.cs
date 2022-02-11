using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Serilog;

namespace Rocket.Surgery.LaunchPad.Serilog;

/// <summary>
///     Delegate SerilogConventionDelegate
/// </summary>
/// <param name="context">The context.</param>
/// <param name="services">The services.</param>
/// <param name="configuration">The configuration.</param>
/// <param name="loggerConfiguration">The logger configuration.</param>
[PublicAPI]
public delegate void SerilogConvention(
    IConventionContext context, IServiceProvider services, IConfiguration configuration, LoggerConfiguration loggerConfiguration
);
