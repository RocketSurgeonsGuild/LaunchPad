using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Time.Testing;

using NodaTime.Extensions;

using Rocket.Surgery.LaunchPad.Primitives.Joints;

namespace Rocket.Surgery.LaunchPad.Testing;

/// <summary>
///     A fake clock joint used during unit testing
/// </summary>
/// <remarks>
///     <para>
///         Runs before the <see cref="TimeJoint" /> so the fake time provider, and the clock built from it, are the ones registered.
///     </para>
///     <para>
///         The start and the step of the fake clock are read from the <see cref="FakeTimeOptions" /> in the joint context, and
///         default when none were set. This stays out of the constructor until Dovetail resolves constructor parameters of
///         exported joints from the context.
///     </para>
/// </remarks>
[PublicAPI]
[UnitTestJoint]
[DovetailExport]
[BeforeJoint<TimeJoint>]
[DovetailCategory(DovetailCategory.Core)]
public class FakeTimeJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(services);

        var options = context.Get<FakeTimeOptions>() ?? new FakeTimeOptions();

        services.RemoveAll<TimeProvider>();
        services.AddSingleton(new FakeTimeProvider(DateTimeOffset.FromUnixTimeSeconds(options.UnixTimeSeconds)) { AutoAdvanceAmount = options.AdvanceBy.ToTimeSpan(), });
        services.AddSingleton<TimeProvider>(sp => sp.GetRequiredService<FakeTimeProvider>());
        services.AddSingleton(s => s.GetRequiredService<TimeProvider>().ToClock());
    }
}
