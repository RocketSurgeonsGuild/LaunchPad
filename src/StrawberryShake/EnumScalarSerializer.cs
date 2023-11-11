using StrawberryShake.Serialization;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

/// <summary>
/// Base class for  enum serializers
/// </summary>
/// <typeparam name="TRuntime"></typeparam>
public abstract class EnumScalarSerializer<TRuntime> : ScalarSerializer<int, TRuntime>
    where TRuntime : struct
{
    /// <summary>
    /// Constructor for enum serializers
    /// </summary>
    /// <param name="typeName"></param>
    protected EnumScalarSerializer(string typeName) : base(typeName)
    {
    }

    /// <inheritdoc />
    public override TRuntime Parse(int serializedValue) => (TRuntime)Enum.ToObject(typeof(TRuntime), serializedValue);

    /// <inheritdoc />
    protected override int Format(TRuntime runtimeValue) => (int)(object)runtimeValue;
}
