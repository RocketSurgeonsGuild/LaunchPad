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
///         The start and the step of the fake clock come from the <see cref="FakeTimeOptions" /> that Dovetail resolves from the joint
///         context when it creates the joint, and default when none were set.
///     </para>
/// </remarks>
/// <param name="options">The options, or <see langword="null" /> for the defaults.</param>
[PublicAPI]
[UnitTestJoint]
[DovetailExport]
[BeforeJoint<TimeJoint>]
[DovetailCategory(DovetailCategory.Core)]
public class FakeTimeJoint(FakeTimeOptions? options = null) : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(services);

        services.RemoveAll<TimeProvider>();
        services.AddSingleton(new FakeTimeProvider(DateTimeOffset.FromUnixTimeSeconds(_options.UnixTimeSeconds)) { AutoAdvanceAmount = _options.AdvanceBy.ToTimeSpan(), });
        services.AddSingleton<TimeProvider>(sp => sp.GetRequiredService<FakeTimeProvider>());
        services.AddSingleton(s => s.GetRequiredService<TimeProvider>().ToClock());
    }

    private readonly FakeTimeOptions _options = options ?? new FakeTimeOptions();
}
