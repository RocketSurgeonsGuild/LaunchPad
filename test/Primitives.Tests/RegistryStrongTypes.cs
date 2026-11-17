using Rocket.Surgery.LaunchPad.StrongTypes;

using SuperStrong.Types;

namespace Primitives.Tests;

// Strong types of a referenced assembly, for StrongTypeRegistryGeneratorTests: this assembly references SuperStrong, so the registry reads it.

/// <summary>The rules of a score: its value is a number, so the registry describes it with <c>CreateNumber</c>.</summary>
public sealed class ScoreRules : IStrongTypeTemplate<int>
{
    public static StrongTypeDefinition<int> Define() => StrongType.Define<int>().HasMinValue(0).HasMaxValue(10);
}

[StrongType<int, ScoreRules>]
public sealed partial class Score;

/// <summary>Rules carrying a format: the registry takes it from the rules of a strong type that has none of its own.</summary>
[StrongTypeFormat("ticket")]
public sealed class TicketRules : IStrongTypeTemplate<string>
{
    public static StrongTypeDefinition<string> Define() => StrongType.Define<string>().HasLength(6);
}

[StrongType<string, TicketRules>]
public sealed partial class Ticket;

/// <summary>Not public: the application's registry can't name it, so it isn't listed.</summary>
[StrongType<string, TicketRules>]
internal sealed partial class InternalTicket;
