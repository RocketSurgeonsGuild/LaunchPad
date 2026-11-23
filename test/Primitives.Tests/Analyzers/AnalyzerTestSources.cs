using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;

namespace Primitives.Tests.Analyzers;

/// <summary>
///     What an analyzer test compiles besides its own source: the attributes the generator writes into a project (read from the analyzer
///     assembly, where they are embedded) and the <c>EmbeddedAttribute</c> they carry, which the generator adds the same way.
/// </summary>
internal static class AnalyzerTestSources
{
    public const string EmbeddedAttribute = """
                                            namespace Microsoft.CodeAnalysis
                                            {
                                                internal sealed class EmbeddedAttribute : global::System.Attribute;
                                            }
                                            """;

    /// <summary>The attributes, as the generator writes them.</summary>
    public static string Attributes { get; } = ReadAttributes();

    /// <summary>
    ///     A compilation of <paramref name="source" /> with the attributes, running <typeparamref name="TAnalyzer" />. The attributes go in as
    ///     generated files, like the generator's output, so the analyzers skip them.
    /// </summary>
    public static GeneratorTestContextBuilder Builder<TAnalyzer>(string source)
        where TAnalyzer : DiagnosticAnalyzer, new() =>
        GeneratorTestContextBuilder
           .Create()
           .WithLanguageVersion(LanguageVersion.Preview)
           .WithDocumentationMode(DocumentationMode.Diagnose)
           .AddReferences(typeof(Rocket.Surgery.LaunchPad.Primitives.IDomainPrimitive<>).Assembly)
           .AddSource("PrimitiveAttributes.g.cs", Attributes)
           .AddSource("EmbeddedAttribute.g.cs", EmbeddedAttribute)
           .AddSource("Input.cs", source)
           .WithAnalyzer<TAnalyzer>();

    /// <summary>Runs <typeparamref name="TAnalyzer" /> on <paramref name="source" />.</summary>
    public static Task<GeneratorTestResults> Analyze<TAnalyzer>(string source, Func<GeneratorTestContextBuilder, GeneratorTestContextBuilder>? configure = null)
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        var builder = Builder<TAnalyzer>(source);
        return ( configure?.Invoke(builder) ?? builder ).Build().GenerateAsync(CancellationToken.None);
    }

    /// <summary>Runs <typeparamref name="TAnalyzer" /> and <typeparamref name="TFix" /> on <paramref name="source" />.</summary>
    public static Task<GeneratorTestResults> Fix<TAnalyzer, TFix>(string source, Func<GeneratorTestContextBuilder, GeneratorTestContextBuilder>? configure = null)
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TFix : CodeFixProvider, new() =>
        Analyze<TAnalyzer>(source, builder => ( configure?.Invoke(builder) ?? builder ).WithCodeFix<TFix>());

    /// <summary>
    ///     The LPAD diagnostics the analyzer reported, each as <c>ID: text</c>, the text being what its location spans (the type name, the
    ///     attribute, the expression), or <c>ID</c> alone for a diagnostic without a location. In source order.
    /// </summary>
    public static string[] Diagnostics(GeneratorTestResults results) =>
        [.. results.AnalyzerResults.Values
               .SelectMany(r => r.Diagnostics)
               // AD0001: an analyzer threw, which an empty list must not hide.
               .Where(d => d.Id.StartsWith("LPAD", StringComparison.Ordinal) || d.Id == "AD0001")
               // The harness can report a diagnostic more than once.
               .GroupBy(d => (d.Id, d.Location.SourceTree?.FilePath, d.Location.SourceSpan))
               .Select(g => g.First())
               .OrderBy(d => d.Location.SourceSpan.Start)
               .ThenBy(d => d.Id, StringComparer.Ordinal)
               .Select(Describe)];

    /// <summary>The diagnostics of one ID, with their messages, to check a message.</summary>
    public static string[] Messages(GeneratorTestResults results, string id) =>
        [.. results.AnalyzerResults.Values
               .SelectMany(r => r.Diagnostics)
               .Where(d => d.Id == id)
               .Select(d => d.GetMessage(System.Globalization.CultureInfo.InvariantCulture))
               .Distinct()];

    /// <summary>The titles of the fixes <typeparamref name="TFix" /> offered.</summary>
    public static string[] FixTitles<TFix>(GeneratorTestResults results)
        where TFix : CodeFixProvider =>
        results.CodeFixResults.TryGetValue(typeof(TFix), out var fixes)
            ? [.. fixes.ResolvedFixes.SelectMany(f => f.CodeActions).Select(a => a.CodeAction.Title).Distinct()]
            : [];

    /// <summary>The input source as the fix titled <paramref name="title" /> leaves it, for the first diagnostic it was offered on.</summary>
    public static async Task<string> Fixed<TFix>(GeneratorTestResults results, string title)
        where TFix : CodeFixProvider =>
        ( await FixedAll<TFix>(results, title) ).FirstOrDefault() ?? throw new InvalidOperationException($"No fix titled '{title}' was offered.");

    /// <summary>
    ///     The input source as each fix titled <paramref name="title" /> leaves it, one for each diagnostic it was offered on. Each action is
    ///     applied to the document it was offered on: the harness applies them one after the other, so only its first one shows a fix alone.
    /// </summary>
    public static async Task<string[]> FixedAll<TFix>(GeneratorTestResults results, string title)
        where TFix : CodeFixProvider
    {
        var texts = new List<string>();
        if (!results.CodeFixResults.TryGetValue(typeof(TFix), out var fixes)) return [];
        foreach (var action in fixes.ResolvedFixes.SelectMany(f => f.CodeActions).Where(a => a.CodeAction.Title == title))
        {
            var operations = await action.CodeAction.GetOperationsAsync(CancellationToken.None);
            foreach (var operation in operations)
            {
                if (operation is not ApplyChangesOperation apply) continue;
                var document = apply.ChangedSolution.GetDocument(action.TargetDocument.Id)!;
                texts.Add(( await document.GetTextAsync() ).ToString());
                break;
            }
        }

        return [.. texts];
    }

    private static string Describe(Diagnostic diagnostic) =>
        diagnostic switch
        {
            // The exception, to see what went wrong.
            { Id: "AD0001" } => $"AD0001: {diagnostic.GetMessage(System.Globalization.CultureInfo.InvariantCulture)}",
            { Location.SourceTree: { } tree } => $"{diagnostic.Id}: {tree.GetText().ToString(diagnostic.Location.SourceSpan)}",
            _ => diagnostic.Id,
        };

    private static string ReadAttributes()
    {
        var assembly = typeof(DeclarationAnalyzer).Assembly;
        using var stream = assembly.GetManifestResourceStream("Rocket.Surgery.LaunchPad.Primitives.Sources.PrimitiveAttributes.cs")
         ?? throw new InvalidOperationException("The analyzer assembly does not embed PrimitiveAttributes.cs.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
