using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rocket.Surgery.LaunchPad.Primitives;

/// <summary>
///     The enum converters the <c>SystemTextJsonJoint</c> adds to the <see cref="JsonSerializerOptions" /> it configures.
/// </summary>
/// <remarks>
///     The generator that ships with this package registers a camel case <see cref="JsonStringEnumConverter{TEnum}" /> here, from a module
///     initializer, for every enum a JSON context of the assembly reaches. Generic converters need no dynamic code, so enums are written as camel case
///     strings in a native AOT application as well; an enum no JSON context reaches is written as a number.
/// </remarks>
[PublicAPI]
public static class LaunchPadJsonEnumConverters
{
    private static readonly Lock Gate = new();
    private static readonly Dictionary<Type, JsonConverter> Registered = [];

    /// <summary>
    ///     Registers an enum converter. A later registration for the same type replaces the earlier one.
    /// </summary>
    /// <param name="converter">The converter, usually a <see cref="JsonStringEnumConverter{TEnum}" />.</param>
    public static void Register<TEnum>(JsonStringEnumConverter<TEnum> converter) where TEnum : struct, Enum
    {
        ArgumentNullException.ThrowIfNull(converter);
        lock (Gate)
        {
            Registered[typeof(TEnum)] = converter;
        }
    }

    /// <summary>
    ///     The converters registered so far.
    /// </summary>
    public static IReadOnlyCollection<JsonConverter> Converters
    {
        get
        {
            lock (Gate)
            {
                return [.. Registered.Values];
            }
        }
    }

    /// <summary>
    ///     Adds every registered converter the options don't have yet.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>The options.</returns>
    public static JsonSerializerOptions AddEnumConverters(this JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        foreach (var converter in Converters)
        {
            if (!options.Converters.Contains(converter)) options.Converters.Add(converter);
        }

        return options;
    }
}
