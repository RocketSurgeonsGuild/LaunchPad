using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NodaTime;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Serilog;
using Serilog;

namespace Rocket.Surgery.LaunchPad.Foundation.Conventions;

/// <summary>
///     Convention for working with Newtonsoft Json
/// </summary>
[PublicAPI]
[ExportConvention]
public class NewtonsoftJsonConvention : IServiceConvention, ISerilogConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceProvider services, LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration.Destructure.NewtonsoftJsonTypes();
    }

    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
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