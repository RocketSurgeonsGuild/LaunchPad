using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;

namespace Rocket.Surgery.LaunchPad.Analyzers
{
    [Generator]
    public class MutableGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!( context.SyntaxReceiver is SyntaxReceiver syntaxReceiver ))
            {
                return;
            }

            var compliation = ( context.Compilation as CSharpCompilation )!;

        }

        class SyntaxReceiver : ISyntaxReceiver
        {
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode) { }
        }
    }
}