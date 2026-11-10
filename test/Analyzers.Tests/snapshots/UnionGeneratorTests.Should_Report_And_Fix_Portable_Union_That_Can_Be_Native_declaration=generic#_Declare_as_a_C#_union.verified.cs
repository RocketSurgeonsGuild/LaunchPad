//HintName: 
using System.Runtime.CompilerServices;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core;

public record RocketModel(string Id);

internal union Result<T>(T, Problem) where T : notnull;

// Not reported: it has a member of its own.
[Union]
public partial struct WithMember
{
    public partial WithMember(int value);
    public partial WithMember(string value);
    public bool IsNumber => Value is int;
}