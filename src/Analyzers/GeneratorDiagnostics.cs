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

    public static DiagnosticDescriptor TypeMustLiveInSameProject { get; } = new(
        "LPAD0002",
        "Source Type must be in the same Project as the Receiving Type",
        "{0} Type must be in the same Assembly / Project as the {1} Type",
        "LaunchPad",
        DiagnosticSeverity.Error,
        true
    );
}
