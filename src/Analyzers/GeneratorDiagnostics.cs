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

    public static DiagnosticDescriptor ParameterMustBeAPropertyOfTheRequest { get; } = new(
        "LPAD0003",
        "The parameter must map to a property of the request object",
        "The parameter {0} map to a property of the request {1} object",
        "LaunchPad",
        DiagnosticSeverity.Error,
        true
    );

    public static DiagnosticDescriptor ParameterMustBeSameTypeAsTheRelatedProperty { get; } = new(
        "LPAD0004",
        "The parameter type and property type must match",
        "The parameter {0} type {1} must match the property {2} type {3}",
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

    public static DiagnosticDescriptor ParameterMustExist { get; } = new(
        "LPAD0006",
        "The parameter must exist in order for this request to process",
        "The parameter of type {0} must exist in order to fulfill this request {1}",
        "LaunchPad",
        DiagnosticSeverity.Error,
        true
    );

    public static DiagnosticDescriptor PropertyMustBeSettableForTheRequest { get; } = new(
        "LPAD0007",
        "The property must be settable on a request class, init is not supported.",
        "The property {0} must be settable on a request class {1}, init is not supported",
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