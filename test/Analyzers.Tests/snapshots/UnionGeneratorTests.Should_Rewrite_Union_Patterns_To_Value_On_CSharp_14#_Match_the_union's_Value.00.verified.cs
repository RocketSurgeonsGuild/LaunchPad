//HintName: 
using System.Runtime.CompilerServices;
using Rocket.Surgery.LaunchPad.Primitives;

public record RocketModel(string Id);

[Union]
public readonly partial struct GetRocketResult
{
    public partial GetRocketResult(RocketModel value);
    public partial GetRocketResult(NotFound value);
}

public static class Describer
{
    public static string Describe(GetRocketResult result) => result.Value switch
    {
        RocketModel rocket => rocket.Id,
        NotFound notFound => "missing",
        _ => throw new global::System.Diagnostics.UnreachableException()
    };
}