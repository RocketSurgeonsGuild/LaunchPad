#if CONVENTIONS
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Serilog;

namespace Rocket.Surgery.LaunchPad.Serilog
{
    /// <summary>
    /// Delegate SerilogConventionDelegate
    /// </summary>
    /// <param name="context">The context.</param>
    [PublicAPI]
    public delegate void SerilogConvention(IConventionContext context, IConfiguration configuration, LoggerConfiguration loggerConfiguration);
}
#endif