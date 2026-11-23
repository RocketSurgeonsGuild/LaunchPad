using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;

/// <summary>
///     Checks how a domain primitive is declared: a record (LPAD2003), partial, and in partial types when it is nested (LPAD2001), readonly when it
///     is a struct (LPAD2002), sealed when it is a class (LPAD2019), and without a public constructor that skips its rules (LPAD2012). Only the
///     declarations the user wrote are read.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DeclarationAnalyzer : DiagnosticAnalyzer
{
    /// <summary>Set on LPAD2001 when it is about a type that contains a domain primitive, not the domain primitive itself.</summary>
    internal const string ContainingTypeProperty = "ContainingType";

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        AnalyzerDiagnostics.MustBePartial,
        AnalyzerDiagnostics.StructMustBeReadOnly,
        AnalyzerDiagnostics.MustBeRecord,
        AnalyzerDiagnostics.PublicConstructor,
        AnalyzerDiagnostics.ClassMustBeSealed,
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
        if (context.Symbol is not INamedTypeSymbol type) return;
        AnalyzeContainingType(context, type);
        if (PrimitiveSymbols.GetDeclaration(type) is null) return;
        var declarations = PrimitiveSymbols.UserDeclarations(type, context.CancellationToken);
        if (declarations.IsEmpty) return;

        if (!type.IsRecord)
        {
            // The fix rewrites the whole declaration, so the modifiers aren't reported on top.
            context.ReportDiagnostic(Diagnostic.Create(AnalyzerDiagnostics.MustBeRecord, declarations[0].Identifier.GetLocation(), type.Name));
            return;
        }

        foreach (var declaration in declarations)
        {
            if (!declaration.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                context.ReportDiagnostic(Diagnostic.Create(AnalyzerDiagnostics.MustBePartial, declaration.Identifier.GetLocation(), type.Name, "it"));
            }
        }

        if (type.TypeKind == TypeKind.Struct && !declarations.Any(d => d.Modifiers.Any(SyntaxKind.ReadOnlyKeyword)))
        {
            context.ReportDiagnostic(Diagnostic.Create(AnalyzerDiagnostics.StructMustBeReadOnly, declarations[0].Identifier.GetLocation(), type.Name));
        }

        if (type.TypeKind == TypeKind.Class && !declarations.Any(d => d.Modifiers.Any(SyntaxKind.SealedKeyword)))
        {
            context.ReportDiagnostic(Diagnostic.Create(AnalyzerDiagnostics.ClassMustBeSealed, declarations[0].Identifier.GetLocation(), type.Name));
        }

        foreach (var constructor in type.InstanceConstructors)
        {
            if (constructor.DeclaredAccessibility != Accessibility.Public || constructor.IsImplicitlyDeclared) continue;
            foreach (var reference in constructor.DeclaringSyntaxReferences)
            {
                if (PrimitiveSymbols.IsGenerated(reference.SyntaxTree, context.CancellationToken)) continue;
                var location = constructor.Locations.FirstOrDefault(l => l.SourceTree == reference.SyntaxTree) ?? reference.GetSyntax(context.CancellationToken).GetLocation();
                context.ReportDiagnostic(Diagnostic.Create(AnalyzerDiagnostics.PublicConstructor, location, type.Name));
            }
        }
    }

    /// <summary>
    ///     LPAD2001 on a type that contains a domain primitive, however deeply: the generator writes the primitive's part inside a <c>partial</c>
    ///     declaration of each of its containing types, which doesn't compile when one isn't partial. Reported on each declaration the user wrote
    ///     without <c>partial</c>, once for the type whatever number of domain primitives it contains. Not reported on a domain primitive that
    ///     contains another: its own LPAD2001 or LPAD2003 already asks for <c>partial</c>.
    /// </summary>
    private static void AnalyzeContainingType(SymbolAnalysisContext context, INamedTypeSymbol type)
    {
        if (type.GetTypeMembers().IsEmpty || PrimitiveSymbols.GetDeclaration(type) is not null || FirstNestedPrimitive(type) is not { } nested) return;
        var properties = ImmutableDictionary<string, string?>.Empty.Add(ContainingTypeProperty, "true");
        foreach (var declaration in PrimitiveSymbols.UserDeclarations(type, context.CancellationToken))
        {
            if (declaration.Modifiers.Any(SyntaxKind.PartialKeyword)) continue;
            context.ReportDiagnostic(
                Diagnostic.Create(
                    AnalyzerDiagnostics.MustBePartial,
                    declaration.Identifier.GetLocation(),
                    properties,
                    type.Name,
                    $"the domain primitive '{nested.Name}' it contains"
                )
            );
        }
    }

    private static INamedTypeSymbol? FirstNestedPrimitive(INamedTypeSymbol type)
    {
        foreach (var member in type.GetTypeMembers())
        {
            if (PrimitiveSymbols.GetDeclaration(member) is not null) return member;
            if (FirstNestedPrimitive(member) is { } nested) return nested;
        }

        return null;
    }
}
