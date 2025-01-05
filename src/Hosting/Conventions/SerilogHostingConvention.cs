using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Hosting;

using Serilog;
using Serilog.Core;
using Serilog.Extensions.Hosting;

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
        services.AddSingleton(sp => new DiagnosticContext(sp.GetRequiredService<Logger>()));
        services.AddSingleton<IDiagnosticContext>(sp => sp.GetRequiredService<DiagnosticContext>());
    }
}
