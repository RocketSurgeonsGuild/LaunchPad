using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using SuperStrong.Types;

namespace Primitives.Tests;

internal static class StrongTypeAssert
{
    public static async Task<TStrongType> Accepts<TStrongType, TPrimitive>(TPrimitive value)
        where TStrongType : class, IStrongType<TStrongType, TPrimitive>
        where TPrimitive : notnull
    {
        await TStrongType.TryFrom(value, out var result).Should().Because($"{value} should be accepted").BeTrue();
        await result.Should().NotBeNull();
        await result.AsPrimitive().Should().BeEqualTo(value);
        await TStrongType.From(value).Should().BeEqualTo(result);
        return result;
    }

    public static async Task Rejects<TStrongType, TPrimitive>(TPrimitive value)
        where TStrongType : class, IStrongType<TStrongType, TPrimitive>
        where TPrimitive : notnull
    {
        await TStrongType.TryFrom(value, out var result).Should().Because($"{value} should be rejected").BeFalse();
        await result.Should().BeNull();
        await ( (Action)( () => TStrongType.From(value) ) ).Should().Throw<StrongTypeValidationException>();
    }

    /// <summary>Writes <paramref name="value" /> as <paramref name="json" /> and reads it back, through the source generated context.</summary>
    public static async Task RoundTrips<T>(T value, string json)
    {
        var typeInfo = TypeInfo<T>();
        await JsonSerializer.Serialize(value, typeInfo).Should().BeEqualTo(json);
        var read = JsonSerializer.Deserialize(json, typeInfo);
        if (value is System.Collections.IEnumerable and not string)
        {
            // a collection (a dictionary keyed by a strong type) compares by its items
            await Assert.That(read).IsEquivalentTo(value);
        }
        else
        {
            await read.Should().BeEqualTo(value);
        }
    }

    /// <summary>Reading <paramref name="json" /> fails, since every value read goes through the rules.</summary>
    public static async Task FailsToRead<T>(string json) => await ( (Action)( () => JsonSerializer.Deserialize(json, TypeInfo<T>()) ) ).Should().Throw<Exception>();

    private static JsonTypeInfo<T> TypeInfo<T>() => (JsonTypeInfo<T>)StrongTypesJsonContext.Default.GetTypeInfo(typeof(T))!;
}
