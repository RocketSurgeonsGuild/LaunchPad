using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Serilog;
using System;

namespace Rocket.Surgery.LaunchPad.Serilog
{
    /// <summary>
    /// Implements the <see cref="IConvention" />
    /// </summary>
    /// <seealso cref="IConvention" />
    [PublicAPI]
    public interface ISerilogConvention : IConvention
    {
        /// <summary>
        /// A serilog convention
        /// </summary>
        /// <param name="context"></param>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="loggerConfiguration"></param>
        void Register(IConventionContext context, IServiceProvider services, IConfiguration configuration, LoggerConfiguration loggerConfiguration);
    }
}