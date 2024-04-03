using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Rocket.Surgery.LaunchPad.Analyzers;

internal static class ContextExtensions
{
    public static void AddSourceRelativeTo(this SourceProductionContext context, TypeDeclarationSyntax declaration, string suffix, SourceText sourceText)
    {
        context.AddSource(
            $"{Path.GetFileNameWithoutExtension(declaration.SyntaxTree.FilePath)}_{string.Join("_", declaration.GetParentDeclarationsWithSelf().Reverse().Select(z => z.Identifier.Text))}_{suffix}",
            sourceText
        );
    }
}