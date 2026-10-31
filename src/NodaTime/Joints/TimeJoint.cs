using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using NodaTime;
using NodaTime.TimeZones;

namespace Rocket.Surgery.LaunchPad.NodaTime.Joints;

/// <summary>
///     A joint that registers the clock, time provider and time zone provider.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <remarks>
///     Dovetail resolves the <see cref="NodaTimeOptions" /> from the joint context when it creates the joint, and they default when none were set.
/// </remarks>
/// <param name="options">The options, or <see langword="null" /> for the defaults.</param>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Core)]
public class TimeJoint(NodaTimeOptions? options = null) : IServiceJoint
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

        // Try add so that unit tests can insert fakes
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<IClock>(SystemClock.Instance);
        services.TryAddSingleton<IDateTimeZoneProvider>(new DateTimeZoneCache(_options.DateTimeZoneSource));
    }

    private readonly NodaTimeOptions _options = options ?? new NodaTimeOptions();
}
