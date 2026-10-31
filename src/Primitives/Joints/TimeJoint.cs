using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using NodaTime;
using NodaTime.TimeZones;

namespace Rocket.Surgery.LaunchPad.Primitives.Joints;

/// <summary>
///     A joint that registers the clock, time provider and time zone provider.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <remarks>
///     The <see cref="PrimitiveOptions" /> are read from the joint context when the joint registers, and default when none were set.
///     This stays out of the constructor until Dovetail resolves constructor parameters of exported joints from the context.
/// </remarks>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Core)]
public class TimeJoint : IServiceJoint
{
    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The joint context.</param>
    /// <param name="services">The service collection.</param>
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(services);

        var options = context.Get<PrimitiveOptions>() ?? new PrimitiveOptions();

        // Try add so that unit tests can insert fakes
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<IClock>(SystemClock.Instance);
        services.TryAddSingleton<IDateTimeZoneProvider>(new DateTimeZoneCache(options.DateTimeZoneSource));
    }
}
