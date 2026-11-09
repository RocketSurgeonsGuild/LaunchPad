using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers;

/// <summary>
///     LPAD0012: <c>[Union]</c> doesn't bind because the project has no polyfill. LPAD0013: on C# 15, a match over a union's <c>Value</c> that the
///     union's own patterns cover. LPAD0014: on C# 15, a portable union that can be declared as a native union. See docs/design/union-types.md.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnionAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The suffix of the union generator's hint names, so its output isn't mistaken for a second user declaration.</summary>
    internal const string GeneratedSuffix = ".Union.g.cs";

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        [UnionDiagnostics.UnionAttributeMissing, UnionDiagnostics.RedundantValueSwitch, UnionDiagnostics.PortableUnionCanBeNative,];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(
            start =>
            {
                var compilation = start.Compilation;
                var hasUnionAttribute = compilation
                                       .GetTypesByMetadataName(UnionNames.UnionAttribute)
                                       .Any(type => compilation.IsSymbolAccessibleWithin(type, compilation.Assembly));
                if (!hasUnionAttribute)
                {
                    start.RegisterSyntaxNodeAction(AnalyzeAttribute, SyntaxKind.Attribute);
                }

                if (compilation is CSharpCompilation { LanguageVersion: var languageVersion, } && UnionNames.SupportsUnions(languageVersion))
                {
                    start.RegisterSyntaxNodeAction(AnalyzeSwitchExpression, SyntaxKind.SwitchExpression);
                    start.RegisterSyntaxNodeAction(AnalyzeIsPattern, SyntaxKind.IsPatternExpression);
                    start.RegisterSyntaxNodeAction(AnalyzeIsType, SyntaxKind.IsExpression);
                    start.RegisterSyntaxNodeAction(AnalyzeStruct, SyntaxKind.StructDeclaration);
                }
            }
        );
    }

    private static void AnalyzeAttribute(SyntaxNodeAnalysisContext context)
    {
        var attribute = (AttributeSyntax)context.Node;
        var name = attribute.Name switch
        {
            QualifiedNameSyntax qualified => qualified.Right.Identifier.ValueText,
            AliasQualifiedNameSyntax aliased => aliased.Name.Identifier.ValueText,
            SimpleNameSyntax simple => simple.Identifier.ValueText,
            _ => "",
        };
        if (name is not ( "Union" or "UnionAttribute" )) return;
        if (context.SemanticModel.GetSymbolInfo(attribute, context.CancellationToken).Symbol is { }) return;

        context.ReportDiagnostic(Diagnostic.Create(UnionDiagnostics.UnionAttributeMissing, attribute.GetLocation()));
    }

    private static void AnalyzeSwitchExpression(SyntaxNodeAnalysisContext context)
    {
        var switchExpression = (SwitchExpressionSyntax)context.Node;
        if (!TryGetUnionValueAccess(context.SemanticModel, switchExpression.GoverningExpression, context.CancellationToken, out var access, out var union))
        {
            return;
        }

        var cases = UnionNames.Cases(union).ToArray();
        var arms = switchExpression.Arms;
        for (var i = 0; i < arms.Count; i++)
        {
            var arm = arms[i];
            if (arm.Pattern is DiscardPatternSyntax)
            {
                // Only a last, unguarded discard that throws: on C# 15 the match is exhaustive and the arm can go.
                if (i != arms.Count - 1 || arm.WhenClause is { } || arm.Expression is not ThrowExpressionSyntax) return;
                continue;
            }

            if (!IsCasePattern(context.SemanticModel, arm.Pattern, cases, context.CancellationToken)) return;
        }

        Report(context, access);
    }

    private static void AnalyzeIsPattern(SyntaxNodeAnalysisContext context)
    {
        var isPattern = (IsPatternExpressionSyntax)context.Node;
        if (!TryGetUnionValueAccess(context.SemanticModel, isPattern.Expression, context.CancellationToken, out var access, out var union)) return;
        if (!IsCasePattern(context.SemanticModel, isPattern.Pattern, [.. UnionNames.Cases(union)], context.CancellationToken)) return;

        Report(context, access);
    }

    // `x.Value is RocketModel`, without a designation, is an `is` type test rather than a pattern.
    private static void AnalyzeIsType(SyntaxNodeAnalysisContext context)
    {
        var isExpression = (BinaryExpressionSyntax)context.Node;
        if (!TryGetUnionValueAccess(context.SemanticModel, isExpression.Left, context.CancellationToken, out var access, out var union)) return;
        if (context.SemanticModel.GetTypeInfo(isExpression.Right, context.CancellationToken).Type is not { } type) return;
        if (!IsCaseType(context.SemanticModel.Compilation, type, [.. UnionNames.Cases(union)])) return;

        Report(context, access);
    }

    private static void Report(SyntaxNodeAnalysisContext context, MemberAccessExpressionSyntax access) =>
        context.ReportDiagnostic(Diagnostic.Create(UnionDiagnostics.RedundantValueSwitch, access.GetLocation(), access.Expression.ToString()));

    internal static bool TryGetUnionValueAccess(
        SemanticModel semanticModel,
        ExpressionSyntax expression,
        CancellationToken cancellationToken,
        out MemberAccessExpressionSyntax access,
        out INamedTypeSymbol union
    )
    {
        access = null!;
        union = null!;
        if (expression is not MemberAccessExpressionSyntax { Name.Identifier.ValueText: "Value", } memberAccess) return false;
        if (semanticModel.GetTypeInfo(memberAccess.Expression, cancellationToken).Type is not INamedTypeSymbol type || !UnionNames.IsUnion(type)) return false;

        access = memberAccess;
        union = type;
        return true;
    }

    // A pattern the union's own matching handles the same way: a type, declaration or recursive pattern whose type a case converts to or from.
    private static bool IsCasePattern(SemanticModel semanticModel, PatternSyntax pattern, ITypeSymbol[] cases, CancellationToken cancellationToken)
    {
        var typeSyntax = pattern switch
        {
            DeclarationPatternSyntax declaration => declaration.Type,
            TypePatternSyntax type => type.Type,
            RecursivePatternSyntax { Type: { } type, } => type,
            // `NotFound => …` parses as a constant pattern; it is a type pattern when the name binds to a type.
            ConstantPatternSyntax constant when semanticModel.GetSymbolInfo(constant.Expression, cancellationToken).Symbol is ITypeSymbol =>
                constant.Expression,
            _ => null,
        };
        if (typeSyntax is null) return false;
        var patternType = semanticModel.GetSymbolInfo(typeSyntax, cancellationToken).Symbol as ITypeSymbol
         ?? semanticModel.GetTypeInfo(typeSyntax, cancellationToken).Type;
        return patternType is { } && IsCaseType(semanticModel.Compilation, patternType, cases);
    }

    private static bool IsCaseType(Compilation compilation, ITypeSymbol patternType, ITypeSymbol[] cases)
    {
        return cases.Any(
            caseType =>
            {
                var underlying = caseType is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T, TypeArguments: [var inner,], }
                    ? inner
                    : caseType;
                return SymbolEqualityComparer.Default.Equals(underlying.WithNullableAnnotation(NullableAnnotation.None), patternType.WithNullableAnnotation(NullableAnnotation.None))
                 || compilation.ClassifyConversion(underlying, patternType) is { IsImplicit: true, IsUserDefined: false, }
                 || compilation.ClassifyConversion(patternType, underlying) is { IsImplicit: true, IsUserDefined: false, };
            }
        );
    }

    private static void AnalyzeStruct(SyntaxNodeAnalysisContext context)
    {
        var declaration = (StructDeclarationSyntax)context.Node;
        if (!IsNativeCandidate(declaration)) return;
        if (context.SemanticModel.GetDeclaredSymbol(declaration, context.CancellationToken) is not { } symbol) return;
        if (!symbol.GetAttributes().Any(attribute => attribute.AttributeClass is { } attributeClass && UnionNames.FullName(attributeClass) == UnionNames.UnionAttribute))
        {
            return;
        }

        // One declaration written by the user; the union generator's part doesn't count.
        if (symbol.DeclaringSyntaxReferences.Count(reference => !reference.SyntaxTree.FilePath.EndsWith(GeneratedSuffix, StringComparison.Ordinal)) != 1) return;

        var cases = string.Join(", ", CaseConstructors(declaration).Select(constructor => constructor.ParameterList.Parameters[0].Type!.ToString()));
        context.ReportDiagnostic(Diagnostic.Create(UnionDiagnostics.PortableUnionCanBeNative, declaration.Identifier.GetLocation(), declaration.Identifier.ValueText + declaration.TypeParameterList, cases));
    }

    /// <summary>
    ///     Whether a struct is only <c>[Union]</c> and public partial case constructors, each with one plain parameter: then the native declaration
    ///     says the same thing.
    /// </summary>
    internal static bool IsNativeCandidate(StructDeclarationSyntax declaration) =>
        declaration.BaseList is null
     && declaration.AttributeLists is [{ Attributes: [_,], Target: null, },]
     && declaration.Modifiers.Any(SyntaxKind.PartialKeyword)
     && !declaration.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.RefKeyword) || modifier.IsKind(SyntaxKind.UnsafeKeyword))
     && declaration.Members.Count > 0
     && declaration.Members.All(
            member => member is ConstructorDeclarationSyntax
            {
                AttributeLists.Count: 0,
                ParameterList.Parameters: [{ AttributeLists.Count: 0, Modifiers.Count: 0, Default: null, Type: { }, },],
            } constructor
                 && UnionGenerator.IsPartialDefinition(constructor)
                 && constructor.Modifiers.Any(SyntaxKind.PublicKeyword)
        );

    internal static IEnumerable<ConstructorDeclarationSyntax> CaseConstructors(StructDeclarationSyntax declaration) =>
        declaration.Members.OfType<ConstructorDeclarationSyntax>();
}
