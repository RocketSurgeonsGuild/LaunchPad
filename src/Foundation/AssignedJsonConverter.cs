using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     Reads and writes an <see cref="Assigned{T}" /> as the bare <typeparamref name="T" />: a property that is in the JSON is assigned its
///     value, one that isn't stays unassigned (the property's own <see cref="Assigned{T}.Empty" />).
/// </summary>
/// <remarks>
///     <para>
///         An explicit <see langword="null" /> assigns <see langword="null" /> when <typeparamref name="T" /> can hold it (a reference type or a
///         <see cref="Nullable{T}" />): that is how a client clears a value, and a request whose property can't be null rejects it when it is
///         validated. For a value type that can't be null (an enum, a number) it leaves the property unassigned, as if it weren't sent: it has
///         no value to clear to, and client generators write such an optional property as nullable and send <see langword="null" /> for one
///         that wasn't set.
///     </para>
///     <para>
///         The property tracking generator puts it on every <see cref="Assigned{T}" /> property it writes, closed over the property's type, so
///         no converter is made at runtime and it is safe for trimming and native AOT.
///     </para>
/// </remarks>
/// <typeparam name="T">The type of the value.</typeparam>
[PublicAPI]
public sealed class AssignedJsonConverter<T> : JsonConverter<Assigned<T>>
{
    /// <inheritdoc />
    public override bool HandleNull => true;

    /// <inheritdoc />
    public override Assigned<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return reader.TokenType == JsonTokenType.Null
            ? default(T) is null ? new(default) : Assigned<T>.Empty()
            : new(JsonSerializer.Deserialize(ref reader, options.GetTypeInfo<T>()));
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Assigned<T> value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(options);
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        JsonSerializer.Serialize(writer, value.Value!, options.GetTypeInfo<T>());
    }
}
