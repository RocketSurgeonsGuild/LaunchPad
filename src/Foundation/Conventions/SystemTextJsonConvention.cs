using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NodaTime;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Serilog;
using Serilog;

namespace Rocket.Surgery.LaunchPad.Foundation.Conventions;

/// <summary>
///     MediatRConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
public class SystemTextJsonConvention : IServiceConvention, ISerilogConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IServiceProvider services, IConfiguration configuration, LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration.Destructure.SystemTextJsonTypes();
    }

    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services.AddTransient<IConfigureOptions<JsonSerializerOptions>>(
            _ =>
                new ConfigureNamedOptions<JsonSerializerOptions>(
                    null,
                    options =>
                    {
                        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    }
                )
        );
        services.AddTransient<IPostConfigureOptions<JsonSerializerOptions>>(
            sp =>
                new PostConfigureOptions<JsonSerializerOptions, IDateTimeZoneProvider>(
                    null,
                    sp.GetRequiredService<IDateTimeZoneProvider>(),
                    (options, provider) => options.ConfigureNodaTimeForLaunchPad(provider)
                )
        );
    }
}
