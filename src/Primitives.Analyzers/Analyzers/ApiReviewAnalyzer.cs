using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;

/// <summary>
///     Reviews the public members a user adds to a domain primitive (the generated ones don't count): how many there are (LPAD2014), that they
///     are documented (LPAD2015), and how a factory method is named (LPAD2016). All three are Info by default.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ApiReviewAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The names a method that creates a domain primitive may have.</summary>
    internal static readonly ImmutableHashSet<string> FactoryNames = ImmutableHashSet.Create(StringComparer.Ordinal, "Create", "TryCreate", "Parse", "TryParse");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        AnalyzerDiagnostics.ApiSurfaceBudget,
        AnalyzerDiagnostics.MissingDocumentation,
        AnalyzerDiagnostics.InvalidFactoryName,
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
        if (context.Symbol is not INamedTypeSymbol type || PrimitiveSymbols.GetDeclaration(type) is not { } primitive) return;
        var cancellationToken = context.CancellationToken;
        // The members a user calls: methods, operators, properties and fields; not the accessors of a property (the property counts) nor the
        // constructors (LPAD2012 is about a public one).
        var members = type.GetMembers()
                          .Where(
                               m => m.DeclaredAccessibility == Accessibility.Public
                                && !m.IsImplicitlyDeclared
                                && m switch
                                {
                                    IMethodSymbol method => method.MethodKind is MethodKind.Ordinary or MethodKind.UserDefinedOperator or MethodKind.Conversion,
                                    _ => m.Kind is SymbolKind.Property or SymbolKind.Field,
                                }
                                && !PrimitiveSymbols.IsGenerated(m, cancellationToken)
                           )
                          .ToArray();
        if (members.Length == 0) return;

        var budget = Budget(primitive, members);
        if (members.Length > budget)
        {
            context.ReportDiagnostic(Diagnostic.Create(AnalyzerDiagnostics.ApiSurfaceBudget, type.Locations.FirstOrDefault() ?? Location.None, type.Name, members.Length, budget));
        }

        foreach (var member in members)
        {
            if (member is IMethodSymbol method)
            {
                // Accessors, operators and conversions are documented with what they belong to; overrides inherit their documentation.
                if (method.MethodKind != MethodKind.Ordinary || method.IsOverride) continue;
                if (method.IsStatic && SymbolEqualityComparer.Default.Equals(method.ReturnType, type) && !FactoryNames.Contains(method.Name))
                {
                    context.ReportDiagnostic(Diagnostic.Create(AnalyzerDiagnostics.InvalidFactoryName, method.Locations.FirstOrDefault() ?? Location.None, method.Name, type.Name));
                }
            }

            if (member is IPropertySymbol { IsOverride: true }) continue;
            if (!IsDocumented(member, cancellationToken))
            {
                context.ReportDiagnostic(Diagnostic.Create(AnalyzerDiagnostics.MissingDocumentation, member.Locations.FirstOrDefault() ?? Location.None, member.Name, type.Name));
            }
        }
    }

    /// <summary>
    ///     How many public members a domain primitive of its kind should need: a value object gets one more for each property or field it
    ///     declares, a smart enum, in either form, for each of its members (a static field or property) and each property or field of the data
    ///     and behaviour its members share.
    /// </summary>
    internal static int Budget(PrimitiveDeclaration primitive, IReadOnlyCollection<ISymbol> members) =>
        primitive.Family switch
        {
            PrimitiveFamily.Numeric => 27,
            PrimitiveFamily.StrongId => 15,
            PrimitiveFamily.Date => 23,
            PrimitiveFamily.ValueObject => 20 + members.Count(m => m.Kind is SymbolKind.Property or SymbolKind.Field),
            PrimitiveFamily.SmartEnum => 12 + members.Count(m => m.Kind is SymbolKind.Property or SymbolKind.Field),
            PrimitiveFamily.String or PrimitiveFamily.Unknown or _ => 25,
        };

    /// <summary>
    ///     Whether the member has a documentation comment. The compiler parses them only when the project writes documentation, so a <c>///</c>
    ///     comment counts too when it didn't.
    /// </summary>
    private static bool IsDocumented(ISymbol member, CancellationToken cancellationToken)
    {
        // A positional record's property is documented by the record's <param>.
        if (member.DeclaringSyntaxReferences.Any(r => r.GetSyntax(cancellationToken) is ParameterSyntax)) return true;
        if (!string.IsNullOrWhiteSpace(member.GetDocumentationCommentXml(cancellationToken: cancellationToken))) return true;
        foreach (var reference in member.DeclaringSyntaxReferences)
        {
            var syntax = reference.GetSyntax(cancellationToken);
            // A field's documentation is on its declaration, not on its variable.
            var declaration = syntax is VariableDeclaratorSyntax { Parent.Parent: FieldDeclarationSyntax field } ? field : syntax;
            foreach (var trivia in declaration.GetLeadingTrivia())
            {
                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) || trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia)) return true;
                if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) && trivia.ToString().StartsWith("///", StringComparison.Ordinal)) return true;
            }
        }

        return false;
    }
}
