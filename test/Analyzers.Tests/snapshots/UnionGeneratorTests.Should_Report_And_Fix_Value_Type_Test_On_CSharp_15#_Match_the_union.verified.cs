//HintName: 
using System.Runtime.CompilerServices;
using Rocket.Surgery.LaunchPad.Foundation;

public record RocketModel(string Id);

[Union]
public readonly partial struct GetRocketResult
{
    public partial GetRocketResult(RocketModel value);
    public partial GetRocketResult(Problem value);
}

public static class Describer
{
    public static bool IsRocket(GetRocketResult result) => result is RocketModel;
}