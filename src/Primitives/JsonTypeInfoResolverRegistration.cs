using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Rocket.Surgery.LaunchPad.Primitives;

/// <summary>
///     A JSON type info resolver, usually a source generated <see cref="JsonSerializerContext" />, that the <c>SystemTextJsonJoint</c> puts in the
///     <see cref="JsonSerializerOptions.TypeInfoResolverChain" /> of every <see cref="JsonSerializerOptions" /> built from the service provider, and
///     through them of the ASP.NET Core HTTP and MVC JSON options.
/// </summary>
/// <remarks>
///     Register one with <see cref="LaunchPadJsonServiceCollectionExtensions.AddJsonTypeInfoResolver" />. Resolvers are chained by
///     <see cref="Order" />, lowest first, then in the order they were registered, so the first resolver that knows a type wins.
/// </remarks>
/// <param name="Resolver">The resolver to chain.</param>
/// <param name="Order">Where the resolver goes in the chain: lower comes first. An application's own resolvers default to 0.</param>
[PublicAPI]
public sealed record JsonTypeInfoResolverRegistration(IJsonTypeInfoResolver Resolver, int Order = 0)
{
    /// <summary>
    ///     The order LaunchPad's own resolvers are registered with, behind the application's, so an application can take over any type.
    /// </summary>
    public const int LibraryOrder = 1000;
}
