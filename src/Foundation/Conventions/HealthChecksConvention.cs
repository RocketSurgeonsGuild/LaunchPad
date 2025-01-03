using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Serilog;

namespace Rocket.Surgery.LaunchPad.Foundation.Conventions;

/// <summary>
///     EnvironmentLoggingConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
[ConventionCategory(ConventionCategory.Core)]
public class HealthChecksConvention : IServiceConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services.AddHealthChecks();
    }
}
