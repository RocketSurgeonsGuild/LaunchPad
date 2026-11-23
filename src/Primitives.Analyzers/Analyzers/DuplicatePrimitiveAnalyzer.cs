using System.Collections.Concurrent;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Model;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;

/// <summary>
///     LPAD2013: two domain primitives of the compilation with the same kind and the same rules, which may be one concept written twice. Strong
///     IDs are left out: they are meant to be distinct types of the same shape (an OrderId and a CustomerId are both a Guid), so that one can't
///     be given for the other. Smart enums and value objects are left out too: their members, not their attributes, say what they are.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DuplicatePrimitiveAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [AnalyzerDiagnostics.PossibleDuplicate];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(
            start =>
            {
                var primitives = new ConcurrentBag<(INamedTypeSymbol Type, string Signature)>();
                start.RegisterSymbolAction(
                    c =>
                    {
                        if (c.Symbol is not INamedTypeSymbol type || PrimitiveSymbols.GetDeclaration(type) is not { } primitive) return;
                        if (primitive.Family is PrimitiveFamily.StrongId or PrimitiveFamily.SmartEnum or PrimitiveFamily.ValueObject) return;
                        if (PrimitiveSymbols.IsGenerated(type, c.CancellationToken)) return;
                        primitives.Add((type, Signature(primitive)));
                    },
                    SymbolKind.NamedType
                );
                start.RegisterCompilationEndAction(end => Report(end, primitives));
            }
        );
    }

    private static void Report(CompilationAnalysisContext context, IEnumerable<(INamedTypeSymbol Type, string Signature)> primitives)
    {
        // Ordered by signature, then by name: the first of each signature is the original, every other one is reported as a duplicate of it.
        var ordered = primitives.Select(p => (p.Type, p.Signature, Name: p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)))
                                .OrderBy(p => p.Signature, StringComparer.Ordinal)
                                .ThenBy(p => p.Name, StringComparer.Ordinal)
                                .ToArray();
        for (var i = 1; i < ordered.Length; i++)
        {
            if (ordered[i].Signature != ordered[i - 1].Signature) continue;
            var original = i - 1;
            while (original > 0 && ordered[original - 1].Signature == ordered[i].Signature) original--;
            var duplicate = ordered[i].Type;
            context.ReportDiagnostic(
                Diagnostic.Create(AnalyzerDiagnostics.PossibleDuplicate, duplicate.Locations.FirstOrDefault() ?? Location.None, duplicate.Name, ordered[original].Type.Name)
            );
        }
    }

    /// <summary>
    ///     The kind and the rules of a domain primitive, with their arguments, in an order that doesn't depend on how they are written. Two
    ///     primitives with the same signature check the same things. Whether it is a struct or a class doesn't count, nor the names of their
    ///     <c>[GeneratedRegex]</c> members.
    /// </summary>
    internal static string Signature(PrimitiveDeclaration primitive) =>
        string.Join(
            "|",
            new[] { primitive.Attribute }.Concat(primitive.Rules)
                                         // A [GeneratedRegex] member is a rule too.
                                         .Concat(GeneratedRegexes.Members(primitive.Type).Select(z => z.Attribute))
                                         .Select(Describe)
                                         .OrderBy(s => s, StringComparer.Ordinal)
        );

    private static string Describe(AttributeData attribute)
    {
        var arguments = string.Join(",", attribute.ConstructorArguments.Select(a => a.ToCSharpString()));
        var named = string.Join(",", attribute.NamedArguments.OrderBy(n => n.Key, StringComparer.Ordinal).Select(n => $"{n.Key}={n.Value.ToCSharpString()}"));
        return $"{attribute.AttributeClass!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}({arguments}){{{named}}}";
    }
}
