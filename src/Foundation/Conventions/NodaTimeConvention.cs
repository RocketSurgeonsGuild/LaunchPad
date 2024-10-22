using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NodaTime;
using NodaTime.TimeZones;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Serilog;
using Serilog;

namespace Rocket.Surgery.LaunchPad.Foundation.Conventions;

/// <summary>
///     NodaTimeConvention.
/// </summary>
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
[ConventionCategory(ConventionCategory.Core)]
public class NodaTimeConvention : IServiceConvention, ISerilogConvention
{
    private readonly FoundationOptions _options;

    /// <summary>
    ///     Create the NodaTime convention
    /// </summary>
    /// <param name="options"></param>
    public NodaTimeConvention(FoundationOptions? options = null)
    {
        _options = options ?? new FoundationOptions();
    }

    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceProvider services, LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration.Destructure.NodaTimeTypes(services.GetRequiredService<IDateTimeZoneProvider>());
    }

    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(context);

        services.TryAddSingleton<IClock>(SystemClock.Instance);
        services.TryAddSingleton<IDateTimeZoneProvider>(new DateTimeZoneCache(_options.DateTimeZoneSource));
    }
}
