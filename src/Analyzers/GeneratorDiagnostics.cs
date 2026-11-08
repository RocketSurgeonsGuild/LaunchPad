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

    public static DiagnosticDescriptor ParameterMustBeSameTypeOfObject { get; } = new(
        "LPAD0005",
        "The given declaration must match",
        "The declaration {0} must be a {1}.",
        "LaunchPad",
        DiagnosticSeverity.Error,
        true
    );

    public static DiagnosticDescriptor ValidatorShouldCallGeneratedValidationMethod { get; } = new(
        "LPAD0008",
        "The validator should call the generated method",
        "The validator should call the generated method '{0}'",
        "LaunchPad",
        DiagnosticSeverity.Warning,
        true
    );
}
