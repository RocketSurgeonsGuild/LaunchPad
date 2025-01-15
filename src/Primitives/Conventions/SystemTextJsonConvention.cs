using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NodaTime;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Primitives.Conventions;

/// <summary>
///     MediatRConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
[ConventionCategory(ConventionCategory.Core)]
public class SystemTextJsonConvention : IServiceConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services.AddSingleton<IConfigureOptions<JsonSerializerOptions>>(
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
