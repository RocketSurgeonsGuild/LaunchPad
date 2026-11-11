using System.Text.Json;

using HotChocolate.Features;
using HotChocolate.Language;
using HotChocolate.Text.Json;
using HotChocolate.Types;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Helpers;

/// <summary>
///     String class base type
/// </summary>
/// <typeparam name="TRuntimeType"></typeparam>
/// <remarks>
///     Create the base type
/// </remarks>
/// <param name="name"></param>
public abstract class StringToClassBaseType<TRuntimeType>(string name) : ScalarType<TRuntimeType, StringValueNode>(name, BindingBehavior.Implicit)
    where TRuntimeType : class
{

    /// <summary>
    ///     Method to serialize
    /// </summary>
    /// <param name="baseValue"></param>
    /// <returns></returns>
    protected abstract string Serialize(TRuntimeType baseValue);

    /// <summary>
    ///     Method to try and deserialize
    /// </summary>
    /// <param name="str"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    protected abstract bool TryDeserialize(string str, [NotNullWhen(true)] out TRuntimeType? output);

    /// <inheritdoc />
    protected override TRuntimeType OnCoerceInputLiteral(StringValueNode valueLiteral)
    {
        return TryDeserialize(valueLiteral.Value, out var value)
            ? value
            : throw new LeafCoercionException($"Unable to deserialize string to {Name}", this);
    }

    /// <inheritdoc />
    protected override TRuntimeType OnCoerceInputValue(JsonElement inputValue, IFeatureProvider context)
    {
        return inputValue.GetString() is { } str && TryDeserialize(str, out var value)
            ? value
            : throw new LeafCoercionException($"Unable to deserialize string to {Name}", this);
    }

    /// <inheritdoc />
    protected override void OnCoerceOutputValue(TRuntimeType runtimeValue, ResultElement resultValue) =>
        resultValue.SetStringValue(Serialize(runtimeValue));

    /// <inheritdoc />
    protected override StringValueNode OnValueToLiteral(TRuntimeType runtimeValue) => new(Serialize(runtimeValue));
}
