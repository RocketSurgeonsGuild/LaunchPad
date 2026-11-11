using System.Text.Json;

using HotChocolate.Features;
using HotChocolate.Language;
using HotChocolate.Text.Json;
using HotChocolate.Types;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Helpers;

/// <summary>
///     Additional struct types
/// </summary>
/// <typeparam name="TRuntimeType"></typeparam>
/// <remarks>
///     Create the base type
/// </remarks>
/// <param name="name"></param>
public abstract class IntToStructBaseType<TRuntimeType>(string name) : ScalarType<TRuntimeType, IntValueNode>(name, BindingBehavior.Implicit)
    where TRuntimeType : struct
{

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
    protected override TRuntimeType OnCoerceInputLiteral(IntValueNode valueLiteral)
    {
        return TryDeserialize(valueLiteral.ToInt32(), out var value)
            ? value.Value
            : throw new LeafCoercionException($"Unable to deserialize integer to {Name}", this);
    }

    /// <inheritdoc />
    protected override TRuntimeType OnCoerceInputValue(JsonElement inputValue, IFeatureProvider context)
    {
        return inputValue.TryGetInt32(out var number) && TryDeserialize(number, out var value)
            ? value.Value
            : throw new LeafCoercionException($"Unable to deserialize integer to {Name}", this);
    }

    /// <inheritdoc />
    protected override void OnCoerceOutputValue(TRuntimeType runtimeValue, ResultElement resultValue) =>
        resultValue.SetNumberValue(SerializeOrThrow(runtimeValue));

    /// <inheritdoc />
    protected override IntValueNode OnValueToLiteral(TRuntimeType runtimeValue) => new(SerializeOrThrow(runtimeValue));

    private int SerializeOrThrow(TRuntimeType runtimeValue) =>
        TrySerialize(runtimeValue, out var number)
            ? number.Value
            : throw new LeafCoercionException($"Unable to serialize {Name} to an integer", this);
}
