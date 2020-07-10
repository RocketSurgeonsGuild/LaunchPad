using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.Serilog.Conventions;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

[assembly: Convention(typeof(SerilogHostingConvention))]

namespace Rocket.Surgery.LaunchPad.Serilog.Conventions
{
    /// <summary>
    /// SerilogHostingConvention.
    /// Implements the <see cref="IHostingConvention" />
    /// </summary>
    /// <seealso cref="IHostingConvention" />
    public class SerilogHostingConvention : IHostingConvention
    {
        private readonly IConventionScanner _scanner;
        private readonly ILogger _diagnosticSource;
        private readonly LaunchPadLoggingOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogHostingConvention" /> class.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <param name="options">The options.</param>
        public SerilogHostingConvention(
            IConventionScanner scanner,
            ILogger diagnosticSource,
            LaunchPadLoggingOptions? options = null
        )
        {
            _scanner = scanner;
            _diagnosticSource = diagnosticSource;
            _options = options ?? new LaunchPadLoggingOptions();
        }

        /// <inheritdoc />
        public void Register([NotNull] IHostingConventionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Builder.ConfigureServices(
                (context, services) =>
                {
                    // removes default console loggers and such
                    foreach (var item in services
                       .Where(
                            x => x.ImplementationType?.FullName.StartsWith("Microsoft.Extensions.Logging", StringComparison.Ordinal) == true
                             && x.ImplementationType?.FullName.EndsWith("Provider", StringComparison.Ordinal) == true
                        )
                       .ToArray()
                    )
                    {
                        services.Remove(item);
                    }
                }
            );
            context.Builder.UseSerilog(
                (ctx, loggerConfiguration) =>
                {
                    new SerilogBuilder(
                        _scanner,
                        context.Get<IAssemblyProvider>(),
                        context.Get<IAssemblyCandidateFinder>(),
                        ctx.HostingEnvironment,
                        ctx.Configuration,
                        loggerConfiguration,
                        _diagnosticSource,
                        context.Properties
                    ).Configure();
                },
                _options.PreserveStaticLogger,
                _options.WriteToProviders
            );
        }
    }
}