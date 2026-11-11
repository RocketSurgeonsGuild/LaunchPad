using System.Text.Json;

namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     Builds the <see cref="JsonSerializerOptions.TypeInfoResolverChain" /> of a <see cref="JsonSerializerOptions" /> from registered resolvers.
/// </summary>
[PublicAPI]
public static class LaunchPadJsonSerializerOptionsExtensions
{
    /// <summary>
    ///     Puts the resolvers in front of the options' <see cref="JsonSerializerOptions.TypeInfoResolverChain" />, ordered by
    ///     <see cref="JsonTypeInfoResolverRegistration.Order" />.
    /// </summary>
    /// <remarks>
    ///     LaunchPad never adds the reflection based resolver: a type no registered JSON context knows fails the same way in a JIT application as in
    ///     a native AOT one. A reflection resolver the application put in the chain itself stays where it is. Resolvers already in the chain are not
    ///     added again.
    /// </remarks>
    /// <param name="options">The options to configure.</param>
    /// <param name="registrations">The registered resolvers.</param>
    /// <returns>The options.</returns>
    public static JsonSerializerOptions AddTypeInfoResolvers(this JsonSerializerOptions options, IEnumerable<JsonTypeInfoResolverRegistration> registrations)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(registrations);

        var chain = options.TypeInfoResolverChain;
        var resolvers = registrations
                       .Select((registration, index) => (registration, index))
                       .OrderBy(x => x.registration.Order)
                       .ThenBy(x => x.index)
                       .Select(x => x.registration.Resolver)
                       .Distinct()
                       .Where(resolver => !chain.Contains(resolver))
                       .ToArray();
        for (var i = resolvers.Length - 1; i >= 0; i--)
        {
            chain.Insert(0, resolvers[i]);
        }

        return options;
    }
}
