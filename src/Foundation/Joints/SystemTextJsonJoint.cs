using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Rocket.Surgery.LaunchPad.Foundation.Joints;

/// <summary>
///     A joint that configures <see cref="JsonSerializerOptions" /> the LaunchPad way: camel case names, camel case enums, and a
///     <see cref="JsonSerializerOptions.TypeInfoResolverChain" /> built from every registered <see cref="JsonTypeInfoResolverRegistration" />.
///     Implements the <see cref="IServiceJoint" />
/// </summary>
/// <remarks>
///     <para>
///         Register an application's own JSON context (a source generated <see cref="JsonSerializerContext" />) with
///         <see cref="LaunchPadJsonServiceCollectionExtensions.AddJsonTypeInfoResolver" />. It goes in front of LaunchPad's own resolvers. No
///         reflection based resolver is added, so a type no JSON context knows fails the same way in a JIT application as in a native AOT one.
///     </para>
///     <para>
///         Enums are written as camel case strings by the <see cref="JsonStringEnumConverter{TEnum}" />s the package's generator registers with
///         <see cref="LaunchPadJsonEnumConverters" /> for the enums the JSON contexts reach; any other enum is written as a number.
///     </para>
///     <para>The NodaTime converters are added by the NodaTime package.</para>
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

        services.AddJsonTypeInfoResolver(LaunchPadJsonSerializerContext.Default, JsonTypeInfoResolverRegistration.LibraryOrder);
        services.AddSingleton<IConfigureOptions<JsonSerializerOptions>>(
            _ =>
                new ConfigureNamedOptions<JsonSerializerOptions>(
                    null,
                    options =>
                    {
                        options.AddEnumConverters();
                        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    }
                )
        );
        services.AddSingleton<IPostConfigureOptions<JsonSerializerOptions>>(
            provider =>
                new PostConfigureOptions<JsonSerializerOptions, IEnumerable<JsonTypeInfoResolverRegistration>>(
                    null,
                    provider.GetServices<JsonTypeInfoResolverRegistration>(),
                    (options, registrations) => options.AddTypeInfoResolvers(registrations)
                )
        );
    }
}
