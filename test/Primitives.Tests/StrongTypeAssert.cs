using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using SuperStrong.Types;

namespace Primitives.Tests;

internal static class StrongTypeAssert
{
    public static TStrongType Accepts<TStrongType, TPrimitive>(TPrimitive value)
        where TStrongType : class, IStrongType<TStrongType, TPrimitive>
        where TPrimitive : notnull
    {
        TStrongType.TryFrom(value, out var result).ShouldBeTrue($"{value} should be accepted");
        result.ShouldNotBeNull();
        result.AsPrimitive().ShouldBe(value);
        TStrongType.From(value).ShouldBe(result);
        return result;
    }

    public static void Rejects<TStrongType, TPrimitive>(TPrimitive value)
        where TStrongType : class, IStrongType<TStrongType, TPrimitive>
        where TPrimitive : notnull
    {
        TStrongType.TryFrom(value, out var result).ShouldBeFalse($"{value} should be rejected");
        result.ShouldBeNull();
        Should.Throw<StrongTypeValidationException>(() => TStrongType.From(value));
    }

    /// <summary>Writes <paramref name="value" /> as <paramref name="json" /> and reads it back, through the source generated context.</summary>
    public static void RoundTrips<T>(T value, string json)
    {
        var typeInfo = TypeInfo<T>();
        JsonSerializer.Serialize(value, typeInfo).ShouldBe(json);
        JsonSerializer.Deserialize(json, typeInfo).ShouldBe(value);
    }

    /// <summary>Reading <paramref name="json" /> fails, since every value read goes through the rules.</summary>
    public static void FailsToRead<T>(string json) => Should.Throw<Exception>(() => JsonSerializer.Deserialize(json, TypeInfo<T>()));

    private static JsonTypeInfo<T> TypeInfo<T>() => (JsonTypeInfo<T>)StrongTypesJsonContext.Default.GetTypeInfo(typeof(T))!;
}
