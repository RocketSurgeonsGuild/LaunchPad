using System.Collections.Immutable;
using System.Composition;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Rocket.Surgery.LaunchPad.Json;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     Adds the <c>[JsonSerializable]</c> entries LPAD1030 asks for to a <c>JsonSerializerContext</c> of the project, or creates one beside the
///     operation host when the project has none.
/// </summary>
/// <remarks>
///     Every entry gets a <c>TypeInfoPropertyName</c> named the way LaunchPad's generated context names them (<c>CreateRocket.Request</c> is
///     <c>CreateRocketRequest</c>): without one, the System.Text.Json source generator skips a second type of the same simple name with only
///     SYSLIB1031, and nested <c>X.Request</c> types always share one. The fix picks the project's hand-written partial context (one that
///     already has entries first, then by file); it never edits generated code or a context in a referenced assembly. A created context still has
///     to be chained into the JSON options, with <c>AddJsonTypeInfoResolver</c>.
/// </remarks>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddJsonSerializableCodeFix))]
[Shared]
public sealed class AddJsonSerializableCodeFix : CodeFixProvider
{
    private const string EquivalenceKey = nameof(AddJsonSerializableCodeFix);
    private const string ContextMetadataName = "System.Text.Json.Serialization.JsonSerializerContext";
    private const string SerializableMetadataName = "System.Text.Json.Serialization.JsonSerializableAttribute";

    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = [ApiDiagnostics.JsonMetadataMissing.Id];

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => AllTypesFixer.Instance;

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var compilation = await context.Document.Project.GetCompilationAsync(context.CancellationToken).ConfigureAwait(false);
        if (compilation?.GetTypeByMetadataName(ContextMetadataName) is not { } contextType) return;
        var target = FindContext(context.Document.Project, compilation, contextType, context.CancellationToken);
        foreach (var diagnostic in context.Diagnostics)
        {
            if (!diagnostic.Properties.TryGetValue(JsonMetadataCoverage.TypeProperty, out var type) || type is null) continue;
            // Only a diagnostic in this document: the generator reports by file path, and its span means nothing in another one.
            if (diagnostic.Location.GetLineSpan().Path is { Length: > 0 } path && !IsAt(context.Document, path)) continue;
            var display = JsonMetadataCoverage.Display(type);
            var title = target is { } existing
                ? $"Add [JsonSerializable(typeof({display}))] to {existing.Symbol.Name}"
                : $"Add [JsonSerializable(typeof({display}))] to a new JSON context";
            var span = diagnostic.Location.SourceSpan;
            context.RegisterCodeFix(
                CodeAction.Create(title, ct => AddTypes(context.Document, span, [type], ct), EquivalenceKey),
                diagnostic
            );
        }
    }

    /// <summary>Declares <paramref name="types" /> on the project's context, or on a new one in the namespace at <paramref name="span" />.</summary>
    private static async Task<Solution> AddTypes(Document origin, TextSpan span, IReadOnlyCollection<string> types, CancellationToken cancellationToken)
    {
        var solution = origin.Project.Solution;
        var compilation = await origin.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
        if (compilation?.GetTypeByMetadataName(ContextMetadataName) is not { } contextType
         || compilation.GetTypeByMetadataName(SerializableMetadataName) is not { } serializable
         || await origin.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false) is not { } originModel)
        {
            return solution;
        }

        var symbols = types
                     .Select(type => originModel.GetSpeculativeTypeInfo(0, SyntaxFactory.ParseTypeName(type), SpeculativeBindingOption.BindAsTypeOrNamespace).Type)
                     .Where(symbol => symbol is { TypeKind: not TypeKind.Error })
                     .Select(symbol => symbol!)
                     .Distinct<ITypeSymbol>(SymbolEqualityComparer.Default)
                     .ToArray();
        if (symbols.Length == 0) return solution;

        var target = FindContext(origin.Project, compilation, contextType, cancellationToken);
        return target is { }
            ? await AddToContext(target, serializable, symbols, cancellationToken).ConfigureAwait(false)
            : await CreateContext(origin, span, compilation, symbols, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<Solution> AddToContext(ContextTarget target, INamedTypeSymbol serializable, IEnumerable<ITypeSymbol> symbols, CancellationToken cancellationToken)
    {
        var document = target.Document;
        var declaration = target.Declaration;
        var text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
        if (await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false) is not { } model) return document.Project.Solution;

        // What the context declares already, and the property names it uses, so the entries are neither repeated nor clash.
        var declared = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        var usedNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (var attribute in target.Symbol.GetAttributes().Where(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, serializable)))
        {
            if (attribute.ConstructorArguments is not [{ Value: ITypeSymbol type }, ..]) continue;
            declared.Add(type);
            usedNames.Add(
                attribute.NamedArguments.FirstOrDefault(a => a.Key == "TypeInfoPropertyName").Value.Value as string ?? type.Name
            );
        }

        var position = declaration.SpanStart;
        var attributeName = serializable.ToMinimalDisplayString(model, position);
        if (attributeName.EndsWith("Attribute", StringComparison.Ordinal)) attributeName = attributeName[..^"Attribute".Length];
        var entries = symbols
                     .Where(declared.Add)
                     .Select(symbol => Entry(attributeName, TypeName(model, position, symbol), Unique(PropertyName(symbol), usedNames)))
                     .ToArray();
        if (entries.Length == 0) return document.Project.Solution;

        var newLine = NewLine(text);
        var firstToken = declaration.GetFirstToken();
        var line = text.Lines.GetLineFromPosition(firstToken.SpanStart);
        var lineText = text.ToString(TextSpan.FromBounds(line.Start, firstToken.SpanStart));
        var indent = lineText[..( lineText.Length - lineText.TrimStart().Length )];

        // After the context's last entry; with none, before its first attribute (after any doc comment, which is the first token's trivia).
        var change = target.LastEntry is { } lastEntry
            ? new TextChange(new(lastEntry.Span.End, 0), string.Concat(entries.Select(entry => newLine + indent + entry)))
            : new TextChange(new(firstToken.SpanStart, 0), string.Concat(entries.Select(entry => entry + newLine + indent)));
        return document.WithText(text.WithChanges(change)).Project.Solution;
    }

    private static async Task<Solution> CreateContext(Document origin, TextSpan span, Compilation compilation, IEnumerable<ITypeSymbol> symbols, CancellationToken cancellationToken)
    {
        var node = ( await origin.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false) )?.FindNode(span);
        var ns = node?.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>()?.Name.ToString();
        var name = ContextName(origin.Project, compilation, ns);
        var languageVersion = ( origin.Project.ParseOptions as CSharpParseOptions )?.LanguageVersion ?? LanguageVersion.Latest;
        var fileScoped = languageVersion >= LanguageVersion.CSharp10;
        var newLine = NewLine(await origin.GetTextAsync(cancellationToken).ConfigureAwait(false));
        var indent = ns is null || fileScoped ? "" : "    ";
        var usedNames = new HashSet<string>(StringComparer.Ordinal);

        var source = new StringBuilder();
        source.Append("using System.Text.Json.Serialization;").Append(newLine).Append(newLine);
        if (ns is { })
        {
            source.Append("namespace ").Append(ns);
            source.Append(fileScoped ? ";" + newLine + newLine : newLine + "{" + newLine);
        }

        source.Append(indent)
              .Append("/// <summary>The JSON metadata of the types this project's operations read and write; chain it into the JSON options with <c>AddJsonTypeInfoResolver(")
              .Append(name)
              .Append(".Default)</c>.</summary>")
              .Append(newLine);
        foreach (var symbol in symbols)
        {
            source.Append(indent).Append(Entry("JsonSerializable", symbol.ToDisplayString(QualifiedFormat), Unique(PropertyName(symbol), usedNames))).Append(newLine);
        }

        source.Append(indent).Append("internal sealed partial class ").Append(name).Append(" : JsonSerializerContext");
        source.Append(languageVersion >= LanguageVersion.CSharp12 ? ";" : " { }").Append(newLine);
        if (ns is { } && !fileScoped) source.Append('}').Append(newLine);

        var fileName = name + ".cs";
        var filePath = origin.FilePath is { } path && Path.GetDirectoryName(path) is { Length: > 0 } directory ? Path.Combine(directory, fileName) : null;
        return origin.Project.AddDocument(fileName, SourceText.From(source.ToString(), Encoding.UTF8), origin.Folders, filePath).Project.Solution;
    }

    private static readonly SymbolDisplayFormat QualifiedFormat =
        SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);

    // The name the context's usings allow; a name that only drops the namespaces enclosing the context (Contracts.Rockets.X inside
    // Sample.Server) reads worse than the full one.
    private static string TypeName(SemanticModel model, int position, ITypeSymbol symbol)
    {
        var minimal = symbol.ToMinimalDisplayString(model, position);
        var first = new string([.. minimal.TakeWhile(c => char.IsLetterOrDigit(c) || c == '_' || c == '@')]);
        return first.Length > 0
            && model.GetSpeculativeSymbolInfo(position, SyntaxFactory.IdentifierName(first), SpeculativeBindingOption.BindAsTypeOrNamespace).Symbol is INamespaceSymbol
                ? symbol.ToDisplayString(QualifiedFormat)
                : minimal;
    }

    private static string Entry(string attributeName, string type, string propertyName) =>
        $"[{attributeName}(typeof({type}), TypeInfoPropertyName = \"{propertyName}\")]";

    private static string Unique(string name, HashSet<string> used) => Suffixed(name, used.Add);

    // The name, or the name with the first number from 2 up that is free.
    private static string Suffixed(string name, Func<string, bool> isFree)
    {
        var unique = name;
        for (var i = 2; !isFree(unique); i++) unique = name + i.ToString(System.Globalization.CultureInfo.InvariantCulture);
        return unique;
    }

    /// <summary>
    ///     The property name LaunchPad's generated context gives a type (<c>SerializedTypeName</c> in Foundation.Tasks), through the naming both
    ///     share (<see cref="JsonTypeInfoPropertyName" />).
    /// </summary>
    internal static string PropertyName(ITypeSymbol type)
    {
        var arrays = 0;
        for (; type is IArrayTypeSymbol array; type = array.ElementType) arrays++;
        var chain = new List<INamedTypeSymbol>();
        for (var current = type as INamedTypeSymbol; current is { }; current = current.ContainingType) chain.Insert(0, current);
        return chain.Count == 0
            ? JsonTypeInfoPropertyName.Compose([type.Name], [], arrays)
            : JsonTypeInfoPropertyName.Compose(chain.Select(segment => segment.Name), chain.SelectMany(segment => segment.TypeArguments).Select(PropertyName), arrays);
    }

    private static ContextTarget? FindContext(Project project, Compilation compilation, INamedTypeSymbol contextType, CancellationToken cancellationToken)
    {
        var serializable = compilation.GetTypeByMetadataName(SerializableMetadataName);
        var candidates = new List<ContextTarget>();
        foreach (var type in Types(compilation.Assembly.GlobalNamespace))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (DerivesFrom(type, contextType) && Target(project, type, serializable, cancellationToken) is { } target) candidates.Add(target);
        }

        return candidates
              .OrderByDescending(c => c.HasEntries)
              .ThenBy(c => c.Document.FilePath ?? c.Document.Name, StringComparer.Ordinal)
              .ThenBy(c => c.Declaration.SpanStart)
              .FirstOrDefault();
    }

    // Of a context's hand-written partial declarations, the one that already carries entries, so new ones go after its last; else the first.
    private static ContextTarget? Target(Project project, INamedTypeSymbol type, INamedTypeSymbol? serializable, CancellationToken cancellationToken)
    {
        // The context's [JsonSerializable] entries, by symbol, so an alias or a fully qualified name counts as well.
        var entries = type.GetAttributes()
                          .Where(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, serializable))
                          .Select(a => a.ApplicationSyntaxReference?.GetSyntax(cancellationToken))
                          .OfType<AttributeSyntax>()
                          .ToArray();
        ContextTarget? chosen = null;
        foreach (var reference in type.DeclaringSyntaxReferences)
        {
            if (IsGenerated(reference.SyntaxTree, cancellationToken) || project.GetDocument(reference.SyntaxTree) is not { } document) continue;
            if (reference.GetSyntax(cancellationToken) is not ClassDeclarationSyntax declaration || !declaration.Modifiers.Any(SyntaxKind.PartialKeyword)) continue;
            var lastEntry = LastEntry(declaration, entries);
            if (chosen is null || ( chosen.LastEntry is null && lastEntry is { } )) chosen = new(type, document, declaration, lastEntry, entries.Length > 0);
        }

        return chosen;
    }

    private static AttributeListSyntax? LastEntry(ClassDeclarationSyntax declaration, AttributeSyntax[] entries) =>
        declaration.AttributeLists.LastOrDefault(list => entries.Any(entry => entry.SyntaxTree == list.SyntaxTree && list.Span.Contains(entry.Span)));

    // A context in generated code (LaunchPad's own executable context, another generator's) would lose the entry on the next build.
    private static bool IsGenerated(SyntaxTree tree, CancellationToken cancellationToken)
    {
        var path = tree.FilePath.Replace('\\', '/');
        return path.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".generated.cs", StringComparison.OrdinalIgnoreCase)
            || path.Contains("/obj/")
            || tree.GetRoot(cancellationToken).GetLeadingTrivia().Any(t => t.ToString().Contains("<auto-generated"));
    }

    // The generator reports by file path; a document without one (an in-memory workspace) goes by its name.
    private static bool IsAt(Document document, string path) => ( document.FilePath ?? document.Name ) == path;

    // The project's name, its last segment, as an identifier: Sample.Server gets ServerJsonContext, unless that name is taken.
    private static string ContextName(Project project, Compilation compilation, string? ns)
    {
        var last = ( project.AssemblyName ?? project.Name ).Split('.').Last();
        var identifier = new string([.. last.Where(c => char.IsLetterOrDigit(c) || c == '_')]);
        if (identifier.Length == 0 || char.IsDigit(identifier[0])) identifier = "App" + identifier;
        return Suffixed(identifier + "JsonContext", name => compilation.GetTypeByMetadataName(ns is null ? name : ns + "." + name) is null);
    }

    private static string NewLine(SourceText text) => text.ToString().Contains("\r\n") ? "\r\n" : "\n";

    private static bool DerivesFrom(INamedTypeSymbol type, INamedTypeSymbol baseType)
    {
        for (var current = type.BaseType; current is { }; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType)) return true;
        }

        return false;
    }

    private static IEnumerable<INamedTypeSymbol> Types(INamespaceSymbol ns)
    {
        foreach (var type in ns.GetTypeMembers())
        {
            foreach (var nested in WithNested(type)) yield return nested;
        }

        foreach (var child in ns.GetNamespaceMembers())
        {
            foreach (var type in Types(child)) yield return type;
        }
    }

    private static IEnumerable<INamedTypeSymbol> WithNested(INamedTypeSymbol type)
    {
        yield return type;
        foreach (var nested in type.GetTypeMembers().SelectMany(WithNested)) yield return nested;
    }

    /// <summary>A context's partial declaration to write onto, with its last attribute list that holds an entry, if any.</summary>
    private sealed record ContextTarget(INamedTypeSymbol Symbol, Document Document, ClassDeclarationSyntax Declaration, AttributeListSyntax? LastEntry, bool HasEntries);

    /// <summary>Fix all: one edit per project, so the entries of every diagnostic land on the same context (or the same new one).</summary>
    private sealed class AllTypesFixer : FixAllProvider
    {
        public static AllTypesFixer Instance { get; } = new();

        public override IEnumerable<FixAllScope> GetSupportedFixAllScopes() => [FixAllScope.Document, FixAllScope.Project, FixAllScope.Solution];

        public override async Task<CodeAction?> GetFixAsync(FixAllContext fixAllContext)
        {
            var diagnostics = await Diagnostics(fixAllContext).ConfigureAwait(false);
            var solution = fixAllContext.Solution;
            // Per project: the document of its first diagnostic (where a new context goes) and every missing type.
            var projects = new Dictionary<ProjectId, (DocumentId Document, TextSpan Span, List<string> Types)>();
            foreach (var diagnostic in diagnostics)
            {
                if (diagnostic.Id != ApiDiagnostics.JsonMetadataMissing.Id
                 || !diagnostic.Properties.TryGetValue(JsonMetadataCoverage.TypeProperty, out var type)
                 || type is null
                 || DocumentOf(solution, diagnostic) is not { } document)
                {
                    continue;
                }

                if (!projects.TryGetValue(document.ProjectId, out var entry))
                {
                    entry = (document, diagnostic.Location.SourceSpan, []);
                    projects.Add(document.ProjectId, entry);
                }

                if (!entry.Types.Contains(type)) entry.Types.Add(type);
            }

            return projects.Count == 0 ? null : CodeAction.Create("Add the missing types to the JSON context", Apply, EquivalenceKey);

            async Task<Solution> Apply(CancellationToken cancellationToken)
            {
                var current = solution;
                foreach (var entry in projects.Values)
                {
                    if (current.GetDocument(entry.Document) is not { } origin) continue;
                    current = await AddTypes(origin, entry.Span, entry.Types, cancellationToken).ConfigureAwait(false);
                }

                return current;
            }
        }

        // Only the scopes GetSupportedFixAllScopes offers; an if chain, not a switch, so no analyzer asks for the other scopes as cases.
        private static async Task<IEnumerable<Diagnostic>> Diagnostics(FixAllContext fixAllContext)
        {
            if (fixAllContext.Scope == FixAllScope.Document)
            {
                return fixAllContext.Document is { } document ? await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false) : [];
            }

            if (fixAllContext.Scope == FixAllScope.Project) return await fixAllContext.GetAllDiagnosticsAsync(fixAllContext.Project).ConfigureAwait(false);
            if (fixAllContext.Scope != FixAllScope.Solution) return [];
            var perProject = await Task.WhenAll(fixAllContext.Solution.Projects.Select(fixAllContext.GetAllDiagnosticsAsync)).ConfigureAwait(false);
            return perProject.SelectMany(d => d);
        }

        // The generator reports by file path, so the location usually has no syntax tree.
        private static DocumentId? DocumentOf(Solution solution, Diagnostic diagnostic)
        {
            if (diagnostic.Location.SourceTree is { } tree && solution.GetDocument(tree) is { } document) return document.Id;
            var path = diagnostic.Location.GetLineSpan().Path;
            return string.IsNullOrEmpty(path)
                ? null
                : solution.GetDocumentIdsWithFilePath(path).FirstOrDefault()
                 ?? solution.Projects.SelectMany(p => p.Documents).FirstOrDefault(d => IsAt(d, path))?.Id;
        }
    }
}
