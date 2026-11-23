using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Model;

namespace Primitives.Tests.Analyzers;

/// <summary>
///     What an analyzer test compiles besides its own source: the attributes the generator writes into a project (read from the analyzer
///     assembly, where they are embedded) and the <c>EmbeddedAttribute</c> they carry, which the generator adds the same way.
/// </summary>
/// <remarks>
///     The attributes are added by hand rather than by running the domain primitive generator: the harness builds the workspace the code fixes
///     run in from the input sources alone, without generators, so a fix's semantic model wouldn't see the attributes and would read every
///     domain primitive as an error type. The analyzers see what they would before the generator has run, which they handle (they recognise a
///     domain primitive by its attribute as well as by its interface). <see cref="ErrorsAfterFix{TFix}" /> compiles the fixed code with the
///     generator instead.
/// </remarks>
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

    /// <summary>
    ///     The errors of the project once every fix titled one of <paramref name="titles" /> is applied (one for each diagnostic it was offered on,
    ///     merged like a fix all), compiled the way a real build compiles it: with the domain primitive generator instead of the attributes the test adds
    ///     by hand, so a fix that leaves code the generator's output can't complete fails. The generator leaves alone a domain primitive that isn't
    ///     a partial record, which compiles, so each domain primitive it didn't complete is an error too. Empty when the fixed project compiles
    ///     and the generator completed each domain primitive.
    /// </summary>
    public static async Task<string[]> ErrorsAfterFix<TFix>(GeneratorTestResults results, params string[] titles)
        where TFix : CodeFixProvider
    {
        if (!results.CodeFixResults.TryGetValue(typeof(TFix), out var fixes) || fixes.ResolvedFixes.IsEmpty)
        {
            throw new InvalidOperationException($"{typeof(TFix).Name} offered no fix.");
        }

        var offered = fixes.ResolvedFixes.SelectMany(f => f.CodeActions).ToArray();
        if (titles.FirstOrDefault(t => offered.All(a => a.CodeAction.Title != t)) is { } missing)
        {
            throw new InvalidOperationException($"No fix titled '{missing}' was offered.");
        }

        var actions = offered.Where(a => titles.Contains(a.CodeAction.Title)).ToArray();

        var project = fixes.ResolvedFixes[0].Document.Project;
        var changes = new Dictionary<DocumentId, HashSet<TextChange>>();
        foreach (var action in actions)
        {
            foreach (var operation in await action.CodeAction.GetOperationsAsync(CancellationToken.None))
            {
                if (operation is not ApplyChangesOperation apply) continue;
                foreach (var document in project.Documents)
                {
                    if (apply.ChangedSolution.GetDocument(document.Id) is not { } changed) continue;
                    foreach (var change in await changed.GetTextChangesAsync(document))
                    {
                        if (!changes.TryGetValue(document.Id, out var set)) changes[document.Id] = set = [];
                        set.Add(change);
                    }
                }
            }
        }

        var parseOptions = (CSharpParseOptions)results.InputCompilation.SyntaxTrees[0].Options;
        var trees = new List<SyntaxTree>();
        foreach (var document in project.Documents)
        {
            // The generator writes the attributes and the EmbeddedAttribute itself.
            if (document.Name is "PrimitiveAttributes.g.cs" or "EmbeddedAttribute.g.cs") continue;
            var text = await document.GetTextAsync();
            if (changes.TryGetValue(document.Id, out var set))
            {
                var ordered = new List<TextChange>(set);
                ordered.Sort((a, b) => a.Span.Start.CompareTo(b.Span.Start));
                text = text.WithChanges(ordered);
            }

            trees.Add(CSharpSyntaxTree.ParseText(text, parseOptions, document.Name));
        }

        var known = results.InputCompilation.References.Select(r => r.Display).ToHashSet(StringComparer.Ordinal);
        var compilation = results.InputCompilation
                                 .RemoveAllSyntaxTrees()
                                 .AddSyntaxTrees(trees)
                                 .AddReferences(GeneratedCodeReferences.Where(r => !known.Contains(r.Display)));
        _ = CSharpGeneratorDriver.Create([new DomainPrimitiveGenerator().AsSourceGenerator()], parseOptions: parseOptions)
                                 .RunGeneratorsAndUpdateCompilation(compilation, out var output, out var generatorDiagnostics);
        return [.. generatorDiagnostics.Concat(output.GetDiagnostics())
                                       .Where(d => d.Severity == DiagnosticSeverity.Error)
                                       .Select(d => d.ToString()),
            .. Uncompleted(output, trees)];
    }

    /// <summary>A message for each domain primitive declared in <paramref name="trees" /> that the generator wrote no part of.</summary>
    private static IEnumerable<string> Uncompleted(Compilation output, List<SyntaxTree> trees)
    {
        var seen = new HashSet<ISymbol>(SymbolEqualityComparer.Default);
        foreach (var tree in trees)
        {
            var model = output.GetSemanticModel(tree);
            foreach (var declaration in tree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>())
            {
                if (model.GetDeclaredSymbol(declaration) is not { } type || !seen.Add(type)) continue;
                if (!type.GetAttributes().Any(a => PrimitiveReader.IsPrimitiveAttribute(a.AttributeClass))) continue;
                if (type.DeclaringSyntaxReferences.Any(r => !trees.Contains(r.SyntaxTree))) continue;
                yield return $"The generator did not complete the domain primitive '{type.ToDisplayString()}'.";
            }
        }
    }

    /// <summary>
    ///     A project named <paramref name="projectName" /> of <paramref name="source" />, completed by the domain primitive generator, to reference
    ///     from a test the way an application references a library's domain primitives. Throws when it doesn't compile.
    /// </summary>
    public static async Task<GeneratorTestResults> CompileWithGenerator(string projectName, string source)
    {
        var results = await GeneratorTestContextBuilder
                           .Create()
                           .WithProjectName(projectName)
                           .WithLanguageVersion(LanguageVersion.Preview)
                           .AddReferences(typeof(Rocket.Surgery.LaunchPad.Primitives.IDomainPrimitive<>).Assembly)
                           .AddReferences(GeneratedCodeReferences)
                           .AddSource(projectName + ".cs", source)
                           .WithGenerator<DomainPrimitiveGenerator>()
                           .Build()
                           .GenerateAsync(CancellationToken.None);
        var errors = results.FinalDiagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => d.ToString()).ToArray();
        return errors.Length == 0 ? results : throw new InvalidOperationException(string.Join("\n", errors));
    }

    /// <summary>The assemblies the generated code compiles against besides those the harness adds.</summary>
    private static readonly MetadataReference[] GeneratedCodeReferences =
    [
        .. new[]
            {
                typeof(System.Text.Json.JsonSerializer),
                typeof(System.ComponentModel.TypeConverter),
                typeof(System.ComponentModel.TypeConverterAttribute),
                typeof(System.Collections.Frozen.FrozenDictionary),
                typeof(System.Text.RegularExpressions.Regex),
                typeof(List<>),
            }
           .Select(t => t.Assembly.Location)
           .Distinct(StringComparer.Ordinal)
           .Select(location => MetadataReference.CreateFromFile(location)),
    ];

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
