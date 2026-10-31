using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Joints;

/// <summary>
///     InstrumentationJoint.
///     Implements <see cref="IOpenTelemetryJoint" />
/// </summary>
/// <seealso cref="IOpenTelemetryJoint" />
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Application)]
public class InstrumentationJoint : IOpenTelemetryJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IOpenTelemetryBuilder builder) => builder.WithTracing(b => b.AddHotChocolateInstrumentation());
}
