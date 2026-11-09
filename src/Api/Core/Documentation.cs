using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     Reads an operation's summary and description from the request's XML documentation when the host and <c>[ApiOperation]</c> set none:
///     the request type's own documentation first, then its containing type's (LaunchPad documents <c>GetRocket</c> as well as
///     <c>GetRocket.Request</c>).
/// </summary>
/// <remarks>
///     A request from a referenced assembly is documented only when the compiler was given that assembly's XML documentation file.
/// </remarks>
internal static class Documentation
{
    /// <summary>The metadata the base package's targets put on the referenced assemblies' XML documentation files.</summary>
    public const string MetadataKey = "build_metadata.AdditionalFiles.LaunchPadApiDocumentation";

    private static readonly Regex Whitespace = new(@"\s+", RegexOptions.Compiled);

    /// <summary>
    ///     The type documentation of the referenced assemblies the build handed over. The compiler gives generators documentation only for source
    ///     symbols, so a request from a contracts project would otherwise have none. Each file is parsed once, when it changes.
    /// </summary>
    public static IncrementalValueProvider<EquatableArray<DocumentationEntry>> Provider(IncrementalGeneratorInitializationContext context) =>
        context.AdditionalTextsProvider
               .Combine(context.AnalyzerConfigOptionsProvider)
               .Where(static pair => pair.Right.GetOptions(pair.Left).TryGetValue(MetadataKey, out var value) && string.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
               .Select(static (pair, cancellationToken) => Parse(pair.Left.GetText(cancellationToken)?.ToString()))
               .Collect()
               .Select(static (files, _) => files.SelectMany(f => f).OrderBy(e => e.Id, StringComparer.Ordinal).ToEquatableArray())
               .WithTrackingName("Documentation");

    public static string? Read(INamedTypeSymbol request, string element, IReadOnlyDictionary<string, DocumentationEntry> referenced)
    {
        for (var type = request; type is { }; type = type.ContainingType)
        {
            var text = Element(type.GetDocumentationCommentXml(), element)
                    ?? ( type.GetDocumentationCommentId() is { } id && referenced.TryGetValue(id, out var entry) ? element == "summary" ? entry.Summary : entry.Remarks : null );
            if (text is { Length: > 0 }) return text;
        }

        return null;
    }

    // Only types: an operation's documentation is its request's or its containing type's.
    private static EquatableArray<DocumentationEntry> Parse(string? xml)
    {
        if (string.IsNullOrWhiteSpace(xml)) return EquatableArray<DocumentationEntry>.Empty;
        try
        {
            return XDocument.Parse(xml)
                            .Root?.Element("members")
                            ?.Elements("member")
                             .Select(member => (Id: (string?)member.Attribute("name"), Member: member))
                             .Where(m => m.Id is { } id && id.StartsWith("T:", StringComparison.Ordinal))
                             .Select(m => new DocumentationEntry(m.Id!, Text(m.Member.Element("summary")), Text(m.Member.Element("remarks"))))
                             .Where(e => e.Summary is { } || e.Remarks is { })
                             .ToEquatableArray()
                ?? EquatableArray<DocumentationEntry>.Empty;
        }
        catch (System.Xml.XmlException)
        {
            return EquatableArray<DocumentationEntry>.Empty;
        }
    }

    private static string? Element(string? xml, string element)
    {
        if (string.IsNullOrWhiteSpace(xml)) return null;
        try
        {
            return Text(XDocument.Parse(xml).Root?.Element(element));
        }
        catch (System.Xml.XmlException)
        {
            return null;
        }
    }

    private static string? Text(XElement? node)
    {
        if (node is null) return null;
        var builder = new StringBuilder();
        Append(builder, node);
        var text = Whitespace.Replace(builder.ToString(), " ").Trim();
        return text.Length > 0 ? text : null;
    }

    // Plain text: <see cref="T:A.B"/> becomes B, <see langword="null"/> null, <paramref name="x"/> x, and other elements their text.
    private static void Append(StringBuilder builder, XElement element)
    {
        foreach (var node in element.Nodes())
        {
            switch (node)
            {
                case XText text:
                    builder.Append(text.Value);
                    break;
                case XElement { Name.LocalName: "see" or "seealso" } reference when !reference.Nodes().Any():
                    builder.Append(ReferenceText(reference));
                    break;
                case XElement { Name.LocalName: "paramref" or "typeparamref" } parameter:
                    builder.Append((string?)parameter.Attribute("name"));
                    break;
                case XElement { Name.LocalName: "para" or "br" } paragraph:
                    builder.Append(' ');
                    Append(builder, paragraph);
                    builder.Append(' ');
                    break;
                case XElement child:
                    Append(builder, child);
                    break;
                default:
                    break;
            }
        }
    }

    private static string ReferenceText(XElement reference)
    {
        if ((string?)reference.Attribute("langword") is { } word) return word;
        var target = (string?)reference.Attribute("cref") ?? (string?)reference.Attribute("href") ?? "";
        var name = target.Contains(':') ? target[( target.IndexOf(':') + 1 )..] : target;
        var parenthesis = name.IndexOf('(');
        if (parenthesis >= 0) name = name[..parenthesis];
        var dot = name.LastIndexOf('.');
        return dot >= 0 ? name[( dot + 1 )..] : name;
    }
}

internal sealed record DocumentationEntry(string Id, string? Summary, string? Remarks);
