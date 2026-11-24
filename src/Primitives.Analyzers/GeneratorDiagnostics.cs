using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers;

/// <summary>
///     The diagnostics of the domain primitive generator: the cases it can't complete a type for. The shape of a declaration (partial,
///     readonly, sealed, a record) is checked by the analyzers, LPAD2001 to LPAD2023.
/// </summary>
internal static class GeneratorDiagnostics
{
    public static readonly DiagnosticDescriptor UnsupportedValueType = new(
        "LPAD2101",
        "Unsupported value type",
        "{0} can't hold {1}: {2}",
        "LaunchPad.Primitives",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MoreThanOnePrimitiveAttribute = new(
        "LPAD2102",
        "More than one primitive attribute",
        "{0} has more than one primitive attribute ({1}); a domain primitive has exactly one kind",
        "LaunchPad.Primitives",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor InvalidAttributeArgument = new(
        "LPAD2103",
        "Invalid attribute argument",
        "{0}: {1}",
        "LaunchPad.Primitives",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor SmartEnumMemberClass = new(
        "LPAD2104",
        "Smart enum member without a class of its own",
        "{0}.{1} must be made with new {1}_(value, ...), its own class, which the generator writes",
        "LaunchPad.Primitives",
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor SmartEnumSharedValue = new(
        "LPAD2105",
        "Smart enum members share a value",
        "{0}'s members {1} and {2} have the same value",
        "LaunchPad.Primitives",
        DiagnosticSeverity.Error,
        true
    );

    public static DiagnosticDescriptor Find(string id) => id switch
    {
        "LPAD2101" => UnsupportedValueType,
        "LPAD2102" => MoreThanOnePrimitiveAttribute,
        "LPAD2104" => SmartEnumMemberClass,
        "LPAD2105" => SmartEnumSharedValue,
        _ => InvalidAttributeArgument,
    };
}
