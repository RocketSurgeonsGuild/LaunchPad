using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;

/// <summary>
///     Suppresses CS0282 on a struct smart enum that carries data. Its data are fields of the application's declaration and its value a field of
///     the part the generator writes, so its fields are in two parts, whose order C# doesn't define. Nothing depends on the layout of a smart
///     enum, and the warning lands on whichever part the compiler reads first, often the application's own file.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SmartEnumLayoutSuppressor : DiagnosticSuppressor
{
    /// <summary>The suppression of CS0282 on a struct smart enum.</summary>
    public static SuppressionDescriptor FieldOrder { get; } = new(
        "LPADSPR2001",
        "CS0282",
        "The fields of a struct smart enum are in the part the generator writes and in its declaration, and nothing depends on their order"
    );

    /// <inheritdoc />
    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; } = [FieldOrder];

    /// <inheritdoc />
    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        foreach (var diagnostic in context.ReportedDiagnostics)
        {
            if (diagnostic.Location.SourceTree is not { } tree) continue;
            var model = context.GetSemanticModel(tree);
            var node = tree.GetRoot(context.CancellationToken).FindNode(diagnostic.Location.SourceSpan);
            if (model.GetDeclaredSymbol(node, context.CancellationToken) is INamedTypeSymbol { IsValueType: true } type
             && PrimitiveSymbols.GetDeclaration(type) is { Family: PrimitiveFamily.SmartEnum })
            {
                context.ReportSuppression(Suppression.Create(FieldOrder, diagnostic));
            }
        }
    }
}
