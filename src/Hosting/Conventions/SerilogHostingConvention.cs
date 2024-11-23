using App.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Hosting;
using Rocket.Surgery.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Hosting;
using Serilog.Extensions.Logging;
using ILogger = Serilog.ILogger;

namespace Rocket.Surgery.LaunchPad.Hosting.Conventions;

/// <summary>
///     SerilogHostingConvention.
///     Implements the <see cref="IHostCreatedConvention{IHost}" />
/// </summary>
/// <seealso cref="IHostCreatedConvention&lt;IHost&gt;" />
[PublicAPI]
[ExportConvention]
[ConventionCategory(ConventionCategory.Core)]
public class SerilogHostingConvention : IServiceConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(context);

        // removes default console loggers and such
        foreach (var item in services
                            .Where(
                                 x =>
                                 {
                                     var type = x.IsKeyedService ? x.KeyedImplementationType : x.ImplementationType;
                                     return type?.FullName?.StartsWith("Microsoft.Extensions.Logging", StringComparison.Ordinal) == true
                                      && type.FullName.EndsWith("Provider", StringComparison.Ordinal);
                                 }
                             )
                            .ToArray()
                )
        {
            services.Remove(item);
        }

        services.ActivateSingleton<Logger>();
        services.ActivateSingleton<LoggerProviderCollection>();
        services.AddSingleton(sp => new DiagnosticContext(sp.GetRequiredService<Logger>()));
        services.AddSingleton<IDiagnosticContext>(sp => sp.GetRequiredService<DiagnosticContext>());
    }
}
