using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Primitives;

/// <summary>
///     Registers JSON type info resolvers for the <c>SystemTextJsonJoint</c> to chain into the application's <see cref="JsonSerializerOptions" />.
/// </summary>
[PublicAPI]
public static class LaunchPadJsonServiceCollectionExtensions
{
    /// <summary>
    ///     Adds a JSON type info resolver, usually a source generated <see cref="JsonSerializerContext" />, to the front of the
    ///     <see cref="JsonSerializerOptions.TypeInfoResolverChain" /> of the application's <see cref="JsonSerializerOptions" />, and of the ASP.NET Core
    ///     HTTP and MVC JSON options when LaunchPad's ASP.NET Core package is used.
    /// </summary>
    /// <remarks>
    ///     This is how a native AOT or trimmed application gives LaunchPad the metadata for its own types:
    ///     <c>services.AddJsonTypeInfoResolver(MyJsonContext.Default)</c>, in a Joint or in the host's service configuration.
    ///     Adding the same resolver twice chains it once.
    /// </remarks>
    /// <param name="services">The service collection.</param>
    /// <param name="resolver">The resolver.</param>
    /// <param name="order">Where the resolver goes in the chain: lower comes first. See <see cref="JsonTypeInfoResolverRegistration.LibraryOrder" />.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddJsonTypeInfoResolver(this IServiceCollection services, IJsonTypeInfoResolver resolver, int order = 0)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(resolver);

        if (services.Any(x => x.ServiceType == typeof(JsonTypeInfoResolverRegistration) && x.ImplementationInstance is JsonTypeInfoResolverRegistration r && ReferenceEquals(r.Resolver, resolver)))
            return services;
        services.AddSingleton(new JsonTypeInfoResolverRegistration(resolver, order));
        return services;
    }
}
