using NodaTime.Text;
using StrawberryShake.Serialization;

namespace Rocket.Surgery.LaunchPad.NodaTime.StrawberryShake;

/// <summary>
/// Shared base class for NodaTime string serializers
/// </summary>
/// <typeparam name="TRuntime"></typeparam>
/// <remarks>
/// Based consturctor for NodaTime string serializers
/// </remarks>
/// <param name="pattern"></param>
/// <param name="typeName"></param>
public abstract class NodaTimeStringScalarSerializer<TRuntime>(IPattern<TRuntime> pattern, string typeName) : ScalarSerializer<string, TRuntime>(typeName)
{
    private readonly IPattern<TRuntime> _pattern = pattern;

    /// <inheritdoc />
    public override TRuntime Parse(string serializedValue) => _pattern.Parse(serializedValue).Value;

    /// <inheritdoc />
    protected override string Format(TRuntime runtimeValue) => _pattern.Format(runtimeValue);
}
