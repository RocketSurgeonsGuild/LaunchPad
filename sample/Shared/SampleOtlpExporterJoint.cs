using Joinery.Dovetail;
using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Sample;

/// <summary>
///     Exports the sample's traces and metrics over OTLP when the <c>OTEL_EXPORTER_OTLP_ENDPOINT</c> environment variable is set, as
///     Sample.AppHost sets it for each sample it runs, so the Aspire dashboard shows them. Run on its own, a sample exports nothing. The
///     exporter reads the rest of the standard settings (protocol, headers, service name) itself. The logs go through Serilog's
///     OpenTelemetry sink instead (LaunchPadSerilogOpenTelemetryJoint), as Serilog is the application's logger.
/// </summary>
[LiveJoint]
[DovetailExport]
internal sealed class SampleOtlpExporterJoint : IOpenTelemetryJoint
{
    public void Register(IDovetailContext context, IOpenTelemetryBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")))
        {
            return;
        }

        builder.WithTracing(static tracing => tracing.AddOtlpExporter());
        builder.WithMetrics(static metrics => metrics.AddOtlpExporter());
    }
}
