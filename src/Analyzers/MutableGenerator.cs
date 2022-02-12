using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Analyzers;

/// <summary>
///     Generator to convert an immutable type into a mutable one
/// </summary>
[Generator]
public class MutableGenerator : ISourceGenerator
{
    /// <inheritdoc />
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    /// <inheritdoc />
    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not SyntaxReceiver)
        {
//            return;
        }

//        var compliation = ( context.Compilation as CSharpCompilation )!;
    }

    private class SyntaxReceiver : ISyntaxReceiver
    {
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
        }
    }
}
