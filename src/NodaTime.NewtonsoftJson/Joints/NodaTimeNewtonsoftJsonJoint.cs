using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using NodaTime;

namespace Rocket.Surgery.LaunchPad.Foundation.Joints;

/// <summary>
///     A joint that configures <see cref="JsonSerializerSettings" /> with the NodaTime converters, using the <see cref="IDateTimeZoneProvider" /> that is registered.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <remarks>
///     The time zone provider is resolved when the settings are first built, so it does not matter whether it is registered before or after this joint.
///     The TimeJoint of the NodaTime package registers the provider an application uses by default.
/// </remarks>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Core)]
public class NodaTimeNewtonsoftJsonJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddTransient<IPostConfigureOptions<JsonSerializerSettings>>(
            sp =>
                new PostConfigureOptions<JsonSerializerSettings, IDateTimeZoneProvider>(
                    null,
                    sp.GetRequiredService<IDateTimeZoneProvider>(),
                    (options, provider) => options.ConfigureNodaTimeForLaunchPad(provider)
                )
        );
    }
}
