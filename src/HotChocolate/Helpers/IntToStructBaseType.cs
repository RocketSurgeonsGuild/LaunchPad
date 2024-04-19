using HotChocolate.Language;
using HotChocolate.Types;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Helpers;

/// <summary>
///     Additional struct types
/// </summary>
/// <typeparam name="TRuntimeType"></typeparam>
public abstract class IntToStructBaseType<TRuntimeType> : ScalarType<TRuntimeType, IntValueNode>
    where TRuntimeType : struct
{
    /// <summary>
    ///     Create the base type
    /// </summary>
    /// <param name="name"></param>
    protected IntToStructBaseType(string name) : base(name, BindingBehavior.Implicit)
    {
    }

    /// <summary>
    ///     Method to try and serialize
    /// </summary>
    /// <param name="baseValue"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    protected abstract bool TrySerialize(TRuntimeType baseValue, [NotNullWhen(true)] out int? output);

    /// <summary>
    ///     Method to try and deserialize
    /// </summary>
    /// <param name="val"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    protected abstract bool TryDeserialize(int val, [NotNullWhen(true)] out TRuntimeType? output);

    /// <inheritdoc />
    protected override TRuntimeType ParseLiteral(IntValueNode valueSyntax)
    {
        if (TryDeserialize(valueSyntax.ToInt32(), out var value)) return value.Value;

        throw new SerializationException(
            $"Unable to deserialize integer to {Name}",
            this
        );
    }

    /// <inheritdoc />
    protected override IntValueNode ParseValue(TRuntimeType runtimeValue)
    {
        if (TrySerialize(runtimeValue, out var val)) return new IntValueNode(val.Value);

        throw new SerializationException(
            $"Unable to deserialize integer to {Name}",
            this
        );
    }

    /// <inheritdoc />
    public override IValueNode ParseResult(object? resultValue)
    {
        if (resultValue is null) return NullValueNode.Default;

        if (resultValue is int s) return new IntValueNode(s);

        if (resultValue is TRuntimeType v) return ParseValue(v);

        throw new SerializationException(
            $"Unable to deserialize integer to {Name}",
            this
        );
    }

    /// <inheritdoc />
    public override bool TrySerialize(object? runtimeValue, out object? resultValue)
    {
        if (runtimeValue is null)
        {
            resultValue = null;
            return true;
        }

        if (runtimeValue is TRuntimeType dt && TrySerialize(dt, out var val))
        {
            resultValue = val.Value;
            return true;
        }

        resultValue = null;
        return false;
    }

    /// <inheritdoc />
    public override bool TryDeserialize(object? resultValue, out object? runtimeValue)
    {
        if (resultValue is null)
        {
            runtimeValue = null;
            return true;
        }

        if (resultValue is int str && TryDeserialize(str, out var val))
        {
            runtimeValue = val;
            return true;
        }

        runtimeValue = null;
        return false;
    }
}
