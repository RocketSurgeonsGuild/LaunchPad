//HintName: 
using System.Runtime.CompilerServices;
using Rocket.Surgery.LaunchPad.Primitives;

namespace Sample.Core;

public record RocketModel(string Id);

/// <summary>What GetRocket answers.</summary>
public union GetRocketResult(RocketModel, NotFound);

// Not reported: it has a member of its own.
[Union]
public partial struct WithMember
{
    public partial WithMember(int value);
    public partial WithMember(string value);
    public bool IsNumber => Value is int;
}