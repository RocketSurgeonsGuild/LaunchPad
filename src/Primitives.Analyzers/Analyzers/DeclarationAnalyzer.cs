using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;

// RS1038: the code fixes in this assembly need Workspaces, which the command-line compiler doesn't load; it creates only the analyzers,
// which don't use it.
#pragma warning disable RS1038

/// <summary>
///     Checks how a domain primitive is declared: a record (LPAD2003), partial (LPAD2001), readonly when it is a struct (LPAD2002), sealed when it
///     is a class (LPAD2019), and without a public constructor that skips its rules (LPAD2012). Only the declarations the user wrote are read.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DeclarationAnalyzer : DiagnosticAnalyzer
{
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
        if (context.Symbol is not INamedTypeSymbol type || PrimitiveSymbols.GetDeclaration(type) is null) return;
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
                context.ReportDiagnostic(Diagnostic.Create(AnalyzerDiagnostics.MustBePartial, declaration.Identifier.GetLocation(), type.Name));
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
}
