using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers;

/// <summary>
///     The diagnostics of the domain primitive generator: the cases it can't complete a type for. The shape of a declaration (partial,
///     readonly, sealed, a record) is checked by the analyzers, LPAD2001 to LPAD2022.
/// </summary>
internal static class GeneratorDiagnostics
{
    public static DiagnosticDescriptor UnsupportedValueType { get; } = new(
        "LPAD2101",
        "Unsupported value type",
        "{0} can't hold a {1}: {2}",
        "LaunchPad.Primitives",
        DiagnosticSeverity.Error,
        true
    );

    public static DiagnosticDescriptor MoreThanOnePrimitiveAttribute { get; } = new(
        "LPAD2102",
        "More than one primitive attribute",
        "{0} has more than one primitive attribute ({1}); a domain primitive has exactly one kind",
        "LaunchPad.Primitives",
        DiagnosticSeverity.Error,
        true
    );

    public static DiagnosticDescriptor InvalidAttributeArgument { get; } = new(
        "LPAD2103",
        "Invalid attribute argument",
        "{0}: {1}",
        "LaunchPad.Primitives",
        DiagnosticSeverity.Error,
        true
    );

    public static DiagnosticDescriptor Find(string id) => id switch
    {
        "LPAD2101" => UnsupportedValueType,
        "LPAD2102" => MoreThanOnePrimitiveAttribute,
        _ => InvalidAttributeArgument,
    };
}
