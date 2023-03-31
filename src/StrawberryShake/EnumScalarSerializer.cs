using StrawberryShake.Serialization;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

public abstract class EnumScalarSerializer<TRuntime> : ScalarSerializer<int, TRuntime>
    where TRuntime : struct
{
    protected EnumScalarSerializer(string typeName) : base(typeName)
    {
    }

    public override TRuntime Parse(int serializedValue) => (TRuntime)Enum.ToObject(typeof(TRuntime), serializedValue);
    protected override int Format(TRuntime runtimeValue) => (int)(object)runtimeValue;
}
