using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Serilog;
using System;

namespace Rocket.Surgery.LaunchPad.Serilog
{
    /// <summary>
    /// Delegate SerilogConventionDelegate
    /// </summary>
    /// <param name="context">The context.</param>
    [PublicAPI]
    public delegate void SerilogConvention(IConventionContext context, IServiceProvider services, IConfiguration configuration, LoggerConfiguration loggerConfiguration);
}