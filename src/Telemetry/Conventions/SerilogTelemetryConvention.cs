using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Serilog;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace Rocket.Surgery.LaunchPad.Telemetry.Conventions;

/// <summary>
/// Defines serilog telemetry configuration
/// </summary>
[PublicAPI]
[ConventionCategory(ConventionCategory.Core)]
public partial class SerilogTelemetryConvention : ISerilogConvention, IServiceConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceProvider services, LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration.WriteTo.OpenTelemetry(
            options =>
            {
                services.GetRequiredService<IOptions<OpenTelemetryLoggerOptions>>();
                var di = services.GetRequiredService<IOptionsMonitor<BatchedOpenTelemetrySinkOptions>>();
                options.BatchingOptions.BatchSizeLimit = di.CurrentValue.BatchingOptions.BatchSizeLimit;
                options.BatchingOptions.BufferingTimeLimit = di.CurrentValue.BatchingOptions.BufferingTimeLimit;
                options.BatchingOptions.EagerlyEmitFirstEvent = di.CurrentValue.BatchingOptions.EagerlyEmitFirstEvent;
                options.BatchingOptions.QueueLimit = di.CurrentValue.BatchingOptions.QueueLimit;
                options.BatchingOptions.RetryTimeLimit = di.CurrentValue.BatchingOptions.RetryTimeLimit;
                options.Endpoint = di.CurrentValue.Endpoint;
                options.FormatProvider = di.CurrentValue.FormatProvider;
                options.Headers = di.CurrentValue.Headers;
                options.HttpMessageHandler = di.CurrentValue.HttpMessageHandler;
                options.IncludedData = di.CurrentValue.IncludedData;
                options.LevelSwitch = di.CurrentValue.LevelSwitch;
                options.LogsEndpoint = di.CurrentValue.LogsEndpoint;
                options.OnBeginSuppressInstrumentation = di.CurrentValue.OnBeginSuppressInstrumentation;
                options.Protocol = di.CurrentValue.Protocol;
                options.ResourceAttributes = di.CurrentValue.ResourceAttributes;
                options.RestrictedToMinimumLevel = di.CurrentValue.RestrictedToMinimumLevel;
                options.TracesEndpoint = di.CurrentValue.TracesEndpoint;
            }
        );
    }

    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services.AddOptions<BatchedOpenTelemetrySinkOptions>();
    }
}
