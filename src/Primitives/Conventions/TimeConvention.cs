using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using NodaTime;
using NodaTime.TimeZones;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Primitives.Conventions;

/// <summary>
///     NodaTimeConvention.
/// </summary>
/// <seealso cref="IServiceConvention" />
/// <remarks>
///     Create the NodaTime convention
/// </remarks>
/// <param name="options"></param>
[PublicAPI]
[ExportConvention]
[ConventionCategory(ConventionCategory.Core)]
public class TimeConvention(PrimitiveOptions? options = null) : IServiceConvention
{
    private readonly PrimitiveOptions _options = options ?? new();

    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Try add so that unit tests can insert fakes
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<IClock>(SystemClock.Instance);
        services.TryAddSingleton<IDateTimeZoneProvider>(new DateTimeZoneCache(_options.DateTimeZoneSource));
    }
}
