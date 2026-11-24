using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;

/// <summary>
///     Checks that a value object is immutable: its public properties use <c>init</c> (LPAD2008) and none is an array or a mutable collection
///     (LPAD2018).
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ValueObjectAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    ///     The mutable collections, by namespace and metadata name: compared by name, since a reference assembly and a facade can both declare
    ///     one, which would make a lookup by metadata name ambiguous.
    /// </summary>
    private static readonly ImmutableHashSet<(string Namespace, string Name)> MutableCollections = ImmutableHashSet.Create(
        ("System.Collections.Generic", "List`1"),
        ("System.Collections.Generic", "Dictionary`2"),
        ("System.Collections.Generic", "HashSet`1"),
        ("System.Collections.Generic", "Queue`1"),
        ("System.Collections.Generic", "Stack`1"),
        ("System.Collections.Generic", "LinkedList`1"),
        ("System.Collections.Generic", "SortedDictionary`2"),
        ("System.Collections.Generic", "SortedList`2"),
        ("System.Collections.Generic", "SortedSet`1"),
        ("System.Collections.ObjectModel", "Collection`1"),
        ("System.Collections.ObjectModel", "ObservableCollection`1")
    );

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        AnalyzerDiagnostics.ValueObjectPropertyMustBeInit,
        AnalyzerDiagnostics.ValueObjectMutableCollection,
    ];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(Analyze, SymbolKind.NamedType);
    }

    private static void Analyze(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol type || PrimitiveSymbols.GetDeclaration(type) is not { Family: PrimitiveFamily.ValueObject }) return;
        foreach (var property in type.GetMembers().OfType<IPropertySymbol>())
        {
            if (property.IsStatic || property.DeclaredAccessibility != Accessibility.Public || property.IsImplicitlyDeclared) continue;
            if (PrimitiveSymbols.IsGenerated(property, context.CancellationToken)) continue;

            if (property.SetMethod is { IsInitOnly: false })
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(AnalyzerDiagnostics.ValueObjectPropertyMustBeInit, property.Locations.FirstOrDefault() ?? Location.None, property.Name, type.Name)
                );
            }

            if (IsMutableCollection(property.Type))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        AnalyzerDiagnostics.ValueObjectMutableCollection,
                        TypeLocation(property, context.CancellationToken),
                        property.Name,
                        type.Name,
                        property.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
                    )
                );
            }
        }
    }

    private static bool IsMutableCollection(ITypeSymbol type) =>
        type is IArrayTypeSymbol
     || type is INamedTypeSymbol { IsGenericType: true } named
     && MutableCollections.Contains((named.ContainingNamespace.ToDisplayString(), named.OriginalDefinition.MetadataName));

    /// <summary>The property's type as written: in its declaration, or in the parameter of a positional record.</summary>
    private static Location TypeLocation(IPropertySymbol property, CancellationToken cancellationToken)
    {
        foreach (var reference in property.DeclaringSyntaxReferences)
        {
            switch (reference.GetSyntax(cancellationToken))
            {
                case PropertyDeclarationSyntax declaration:
                    return declaration.Type.GetLocation();
                case ParameterSyntax { Type: { } parameterType }:
                    return parameterType.GetLocation();
                default:
                    break;
            }
        }

        return property.Locations.FirstOrDefault() ?? Location.None;
    }
}
