using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.Composition;
using Rocket.Surgery.LaunchPad.Telemetry;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions;

/// <summary>
///     ProblemDetailsConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
[AfterConvention(typeof(AspNetCoreConvention))]
public class AspNetCoreConventionInstrumentationConvention : IOpenTelemetryConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext conventionContext, IConfiguration configuration, OpenTelemetryBuilder builder)
    {
        builder
           .WithMetrics(z => z.AddAspNetCoreInstrumentation())
           .WithTracing(
                z => z.AddAspNetCoreInstrumentation(
                    options => options.RecordException = true
                )
            );
    }
}
