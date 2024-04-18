using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Telemetry;

namespace Rocket.Surgery.LaunchPad.Hosting.Conventions;

/// <summary>
///     EnvironmentLoggingConvention.
///     Implements the <see cref="IServiceAsyncConvention" />
/// </summary>
/// <seealso cref="IServiceAsyncConvention" />
[PublicAPI]
[ExportConvention]
public class OpenTelemetryConvention : IServiceAsyncConvention
{
    /// <inheritdoc />
    public async ValueTask Register(
        IConventionContext context,
        IConfiguration configuration,
        IServiceCollection services,
        CancellationToken cancellationToken
    ) =>
        await services
             .AddOpenTelemetry()
             .ApplyConventionsAsync(context, cancellationToken)
             .ConfigureAwait(false);
}
