using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NodaTime;
using Serilog;

namespace Rocket.Surgery.LaunchPad.Foundation.Joints;

/// <summary>
///     Joint for working with Newtonsoft Json
/// </summary>
/// <remarks>
///     A service Joint that configures the Newtonsoft Json serializer settings,
///     and a Serilog Joint that destructures the Newtonsoft Json types.
/// </remarks>
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Core)]
public class NewtonsoftJsonJoint : IServiceJoint, ISerilogJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services, LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration.Destructure.NewtonsoftJsonTypes();
    }

    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        services.AddTransient<IConfigureOptions<JsonSerializerSettings>>(
            _ =>
                new ConfigureNamedOptions<JsonSerializerSettings>(
                    null,
                    options => options.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()))
                )
        );
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
