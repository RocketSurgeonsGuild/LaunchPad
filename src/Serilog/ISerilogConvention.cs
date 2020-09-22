#if CONVENTIONS
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Serilog;

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
        /// <param name="configuration"></param>
        /// <param name="loggerConfiguration"></param>
        void Register(IConventionContext context, IConfiguration configuration, LoggerConfiguration loggerConfiguration);
    }
}
#endif