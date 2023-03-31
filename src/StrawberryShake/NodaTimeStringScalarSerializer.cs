using NodaTime.Text;
using StrawberryShake.Serialization;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

public abstract class NodaTimeStringScalarSerializer<TRuntime> : ScalarSerializer<string, TRuntime>
{
    private readonly IPattern<TRuntime> _pattern;

    protected NodaTimeStringScalarSerializer(IPattern<TRuntime> pattern, string typeName) : base(typeName)
    {
        _pattern = pattern;
    }

    public override TRuntime Parse(string serializedValue) => _pattern.Parse(serializedValue).Value;
    protected override string Format(TRuntime runtimeValue) => _pattern.Format(runtimeValue);
}