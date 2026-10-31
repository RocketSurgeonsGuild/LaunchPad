using System.Text.Json;

using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NodaTime;

namespace Rocket.Surgery.LaunchPad.NodaTime.Joints;

/// <summary>
///     A joint that configures <see cref="JsonSerializerOptions" /> with the NodaTime converters, using the <see cref="IDateTimeZoneProvider" /> that is registered.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <remarks>
///     The time zone provider is resolved when the options are first built, so it does not matter whether it is registered before or after this joint.
///     The <see cref="TimeJoint" /> registers the provider an application uses by default.
/// </remarks>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Core)]
public class NodaTimeSystemTextJsonJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IPostConfigureOptions<JsonSerializerOptions>>(
            sp =>
                new PostConfigureOptions<JsonSerializerOptions, IDateTimeZoneProvider>(
                    null,
                    sp.GetRequiredService<IDateTimeZoneProvider>(),
                    (options, provider) => options.ConfigureNodaTimeForLaunchPad(provider)
                )
        );
    }
}
