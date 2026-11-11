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
    public static string Describe(GetRocketResult result) => result switch
    {
        RocketModel rocket => rocket.Id,
        NotFound => "missing",
        Problem problem => problem.Detail ?? "problem",
    };

    // Not reported: the discard does more than throw, and string isn't a case.
    public static string Keep(GetRocketResult result) => result.Value switch
    {
        RocketModel rocket => rocket.Id,
        _ => "other",
    };

    public static bool KeepToo(GetRocketResult result) => result.Value is string;
}