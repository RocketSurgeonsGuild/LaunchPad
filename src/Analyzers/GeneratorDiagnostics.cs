using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Analyzers;

internal static class GeneratorDiagnostics
{
    public static DiagnosticDescriptor MustBePartial { get; } = new(
        "LPAD0001",
        "Type must be made partial",
        "Type {0} must be made partial.",
        "LaunchPad",
        DiagnosticSeverity.Error,
        true
    );

    public static DiagnosticDescriptor MemberCannotBeCopiedFromMetadata { get; } = new(
        "LPAD0002",
        "Member of a type from another assembly cannot be copied",
        "{0} of {1} is not copied to {2}: from a type in another assembly InheritFrom copies only public properties with a set or init accessor and public fields that are not read only",
        "LaunchPad",
        DiagnosticSeverity.Warning,
        true
    );

    public static DiagnosticDescriptor ParameterMustBeSameTypeOfObject { get; } = new(
        "LPAD0005",
        "The given declaration must match",
        "The declaration {0} must be a {1}.",
        "LaunchPad",
        DiagnosticSeverity.Error,
        true
    );
}
