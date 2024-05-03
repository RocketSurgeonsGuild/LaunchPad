using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Serilog;

namespace Rocket.Surgery.LaunchPad.Serilog;

/// <summary>
///     Delegate SerilogConventionDelegate
/// </summary>
/// <param name="context">The context.</param>
/// <param name="configuration">The configuration.</param>
/// <param name="services">The services.</param>
/// <param name="loggerConfiguration">The logger configuration.</param>
[PublicAPI]
public delegate void SerilogConvention(IConventionContext context, IConfiguration configuration, IServiceProvider services, LoggerConfiguration loggerConfiguration);
