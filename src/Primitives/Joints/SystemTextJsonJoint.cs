using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Rocket.Surgery.LaunchPad.Primitives.Joints;

/// <summary>
///     A joint that configures <see cref="JsonSerializerOptions" /> the LaunchPad way: camel case names and camel case enums.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <remarks>
///     The NodaTime converters are added by the NodaTime package.
/// </remarks>
/// <seealso cref="IServiceJoint" />
[PublicAPI]
[DovetailExport]
[DovetailCategory(DovetailCategory.Core)]
public class SystemTextJsonJoint : IServiceJoint
{
    /// <inheritdoc />
    public void Register(IDovetailContext context, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

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
    }
}
