using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenTelemetry;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Logging;

namespace Rocket.Surgery.LaunchPad.Telemetry.Conventions;

/// <summary>
///     Defines default telemetry convention
/// </summary>
[PublicAPI]
[ExportConvention]
[ConventionCategory(ConventionCategory.Core)]
public class TelemetryConvention : ILoggingConvention, IServiceAsyncConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, ILoggingBuilder builder) => builder.AddOpenTelemetry();

    /// <inheritdoc />
    public ValueTask Register(IConventionContext context, IConfiguration configuration, IServiceCollection services, CancellationToken cancellationToken)
    {
        var builder = new OpenTelemetrySdkBuilder(services);
        return context
           .RegisterConventions(
                e => e
                    .AddHandler<IOpenTelemetryConvention>(convention => convention.Register(context, context.Configuration, builder))
                    .AddHandler<OpenTelemetryConvention>(convention => convention(context, context.Configuration, builder))
                    .AddHandler<IOpenTelemetryAsyncConvention>(convention => convention.Register(context, context.Configuration, builder, cancellationToken))
                    .AddHandler<OpenTelemetryAsyncConvention>(convention => convention(context, context.Configuration, builder, cancellationToken))
                    .AddHandler<IOpenTelemetryMetricsConvention>(convention => builder.WithMetrics(b => convention.Register(context, context.Configuration, b)))
                    .AddHandler<OpenTelemetryMetricsConvention>(convention => builder.WithMetrics(b => convention(context, context.Configuration, b)))
                    .AddHandler<IOpenTelemetryTracingConvention>(convention => builder.WithTracing(b => convention.Register(context, context.Configuration, b)))
                    .AddHandler<OpenTelemetryTracingConvention>(convention => builder.WithTracing(b => convention(context, context.Configuration, b)))
                    .AddHandler<IOpenTelemetryLoggingConvention>(convention => builder.WithLogging(b => convention.Register(context, context.Configuration, b)))
                    .AddHandler<OpenTelemetryLoggingConvention>(convention => builder.WithLogging(b => convention(context, context.Configuration, b)))
                    .AddHandler<IOpenTelemetryResourceConvention>(convention => builder.ConfigureResource(b => convention.Register(context, context.Configuration, b)))
                    .AddHandler<OpenTelemetryResourceConvention>(convention => builder.ConfigureResource(b => convention(context, context.Configuration, b)))
            );
    }

    private sealed class OpenTelemetrySdkBuilder(IServiceCollection services) : IOpenTelemetryBuilder
    {
        public IServiceCollection Services { get; } = services;
    }
}
