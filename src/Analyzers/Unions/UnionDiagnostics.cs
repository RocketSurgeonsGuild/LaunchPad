using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Analyzers;

/// <summary>The names union detection matches by (docs/design/union-types.md, section 4): never symbols, since every assembly before .NET 11 has its own copy.</summary>
internal static class UnionNames
{
    public const string UnionAttribute = "System.Runtime.CompilerServices.UnionAttribute";
    public const string IUnion = "System.Runtime.CompilerServices.IUnion";

    /// <summary>Whether a type is a union: <c>[Union]</c> or <c>IUnion</c>, by name. Roslyn hides <c>[Union]</c> on native and metadata unions.</summary>
    public static bool IsUnion(ITypeSymbol? type)
    {
        if (type is null) return false;
        foreach (var attribute in type.GetAttributes())
        {
            if (attribute.AttributeClass is { } attributeClass && FullName(attributeClass) == UnionAttribute) return true;
        }

        foreach (var @interface in type.AllInterfaces)
        {
            if (FullName(@interface) == IUnion) return true;
        }

        return false;
    }

    /// <summary>The case types: the parameter types of the public one-parameter instance constructors.</summary>
    public static IEnumerable<ITypeSymbol> Cases(INamedTypeSymbol union) =>
        union
           .InstanceConstructors
           .Where(constructor => constructor is { DeclaredAccessibility: Accessibility.Public, Parameters.Length: 1, })
           .Select(constructor => constructor.Parameters[0].Type)
           .Distinct(SymbolEqualityComparer.Default)
           .Cast<ITypeSymbol>();

    /// <summary>Whether the language version gives unions their own patterns and conversions (C# 15).</summary>
    public static bool SupportsUnions(Microsoft.CodeAnalysis.CSharp.LanguageVersion languageVersion) =>
        // Roslyn 5.0 has no CSharp15 member; preview, and C# 15 in later compilers, are above C# 14 once mapped.
        (int)Microsoft.CodeAnalysis.CSharp.LanguageVersionFacts.MapSpecifiedToEffectiveVersion(languageVersion)
      > (int)Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp14;

    public static string FullName(ITypeSymbol type) => type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat.WithGenericsOptions(SymbolDisplayGenericsOptions.None));
}

internal static class UnionDiagnostics
{
    private const string Category = "LaunchPad.Unions";

    public static DiagnosticDescriptor MustBePartial { get; } = new(
        "LPAD0001",
        "Type must be made partial",
        "Type {0} must be made partial.",
        "LaunchPad",
        DiagnosticSeverity.Error,
        true
    );

    public static DiagnosticDescriptor UnionMustBeStruct { get; } = new(
        "LPAD0010",
        "A portable union must be a struct",
        "Union {0} declares partial case constructors, so LaunchPad implements it, and it must be a partial struct, as a C# union is",
        Category,
        DiagnosticSeverity.Error,
        true
    );

    public static DiagnosticDescriptor UnionCaseConstructorMustTakeOneParameter { get; } = new(
        "LPAD0011",
        "A union case constructor takes exactly one parameter",
        "A partial constructor of union {0} declares a case and must take exactly one parameter, the case value; it takes {1}",
        Category,
        DiagnosticSeverity.Error,
        true
    );

    public static DiagnosticDescriptor UnionAttributeMissing { get; } = new(
        "LPAD0012",
        "UnionAttribute isn't available",
        "System.Runtime.CompilerServices.UnionAttribute isn't available before .NET 11: reference the Polyfill or PolySharp package, or use the LaunchPad SDK, which adds Polyfill",
        Category,
        DiagnosticSeverity.Error,
        true
    );

    public static DiagnosticDescriptor RedundantValueSwitch { get; } = new(
        "LPAD0013",
        "Match the union, not its Value",
        "{0} is a union and C# 15 matches its cases directly: match {0} instead of its Value, without the discard arm, so the compiler checks the match is exhaustive",
        Category,
        DiagnosticSeverity.Warning,
        true
    );

    public static DiagnosticDescriptor PortableUnionCanBeNative { get; } = new(
        "LPAD0014",
        "Portable union can be a native union",
        "Union {0} can be declared as a C# union: union {0}({1});",
        Category,
        DiagnosticSeverity.Info,
        true
    );
}
