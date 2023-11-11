using NodaTime.Text;
using StrawberryShake.Serialization;

namespace Rocket.Surgery.LaunchPad.StrawberryShake;

/// <summary>
/// Shared base class for NodaTime string serializers
/// </summary>
/// <typeparam name="TRuntime"></typeparam>
public abstract class NodaTimeStringScalarSerializer<TRuntime> : ScalarSerializer<string, TRuntime>
{
    private readonly IPattern<TRuntime> _pattern;

    /// <summary>
    /// Based consturctor for NodaTime string serializers
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="typeName"></param>
    protected NodaTimeStringScalarSerializer(IPattern<TRuntime> pattern, string typeName) : base(typeName)
    {
        _pattern = pattern;
    }
    
    /// <inheritdoc />
    public override TRuntime Parse(string serializedValue) => _pattern.Parse(serializedValue).Value;
    
    /// <inheritdoc />
    protected override string Format(TRuntime runtimeValue) => _pattern.Format(runtimeValue);
}
