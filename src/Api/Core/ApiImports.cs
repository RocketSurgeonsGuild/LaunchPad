using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     Shortens the fully qualified names the emitters write (<c>global::System.Threading.Tasks.Task</c>) into the names a person would write,
///     with the <c>using</c> directives that make them resolve:
///     <list type="bullet">
///         <item>a non-generic type becomes an alias: <c>using Task = global::System.Threading.Tasks.Task;</c> (an attribute under its
///         <c>…Attribute</c> name, written without the suffix);</item>
///         <item>a generic type imports its namespace (an alias can't be generic): <c>using System.Threading.Tasks;</c>;</item>
///         <item>a call into a static <c>…Extensions</c> class becomes an extension call (<c>endpoint.WithName("…")</c>) and imports the class's
///         namespace.</item>
///     </list>
/// </summary>
/// <remarks>
///     Only names whose namespace is in <see cref="Namespaces" /> are shortened: the namespace part of a name has to be known exactly, or
///     <c>global::ProtoBuf.Meta.RuntimeTypeModel.Default</c> (a property) or <c>…ServiceLifetime.Scoped</c> (an enum member) would be taken for
///     a type. The user's own types stay fully qualified. A second name in a file that would shorten to the same name as a first stays
///     qualified. A type the user declares in the host's namespace with the same simple name as an alias would hide it.
/// </remarks>
internal sealed class ApiImports : CSharpSyntaxRewriter
{
    /// <summary>The namespaces whose types generated code may name short.</summary>
    private static readonly HashSet<string> Namespaces =
    [
        with(StringComparer.Ordinal),
        "Asp.Versioning",
        "Asp.Versioning.Builder",
        "ChannelMediator",
        "Foundatio.Mediator",
        "Grpc.Core",
        "Grpc.Net.ClientFactory",
        "HotChocolate",
        "HotChocolate.Authorization",
        "HotChocolate.Execution.Configuration",
        "HotChocolate.Resolvers",
        "HotChocolate.Types",
        "Immediate.Handlers.Shared",
        "MediatR",
        "Mediator",
        "Microsoft.AspNetCore.Authorization",
        "Microsoft.AspNetCore.Builder",
        "Microsoft.AspNetCore.Http",
        "Microsoft.AspNetCore.Http.HttpResults",
        "Microsoft.AspNetCore.Http.Metadata",
        "Microsoft.AspNetCore.Mvc",
        "Microsoft.AspNetCore.Routing",
        "Microsoft.CodeAnalysis",
        "Microsoft.Extensions.DependencyInjection",
        "Microsoft.Extensions.DependencyInjection.Extensions",
        "Microsoft.Extensions.Primitives",
        "ProtoBuf.Grpc",
        "ProtoBuf.Grpc.ClientFactory",
        "ProtoBuf.Grpc.Configuration",
        "ProtoBuf.Meta",
        "Rocket.Surgery.LaunchPad.Api.Generated.GraphQL",
        "Rocket.Surgery.LaunchPad.Api.Generated.Http",
        "Rocket.Surgery.LaunchPad.Api.Generated.MinimalApi",
        "Rocket.Surgery.LaunchPad.Api.Generated.Mvc",
        "System",
        "System.CodeDom.Compiler",
        "System.Collections.Generic",
        "System.Diagnostics.CodeAnalysis",
        "System.Globalization",
        "System.Linq",
        "System.Net",
        "System.Net.Http",
        "System.Net.Http.Json",
        "System.Reflection",
        "System.Runtime.CompilerServices",
        "System.Runtime.Serialization",
        "System.Text.Json",
        "System.Text.Json.Serialization",
        "System.Text.Json.Serialization.Metadata",
        "System.Threading",
        "System.Threading.Tasks",
        "ZeroAlloc.Mediator",
    ];

    // alias name → the type it names; (generic name, arity) → its namespace.
    private readonly Dictionary<string, string> _aliases = [with(StringComparer.Ordinal)];
    private readonly Dictionary<(string Name, int Arity), string> _generics = [];
    private readonly SortedSet<string> _imports = [with(StringComparer.Ordinal)];

    /// <summary>Shortens the names in <paramref name="members" /> and <paramref name="assemblyAttributes" />, and returns them with the using directives they need.</summary>
    public static (IReadOnlyList<MemberDeclarationSyntax> Members, IReadOnlyList<AttributeListSyntax> AssemblyAttributes, IReadOnlyList<UsingDirectiveSyntax> Usings) Apply(
        IEnumerable<MemberDeclarationSyntax> members,
        IEnumerable<AttributeListSyntax> assemblyAttributes,
        IEnumerable<string> usings
    )
    {
        var imports = new ApiImports();
        foreach (var import in usings) imports._imports.Add(import);
        var rewritten = members.Select(m => (MemberDeclarationSyntax)imports.Visit(m)).ToArray();
        var attributes = assemblyAttributes.Select(a => (AttributeListSyntax)imports.Visit(a)).ToArray();
        var directives = imports._imports
                                .OrderBy(n => n.StartsWith("System", StringComparison.Ordinal) ? 0 : 1)
                                .ThenBy(n => n, StringComparer.Ordinal)
                                .Select(n => UsingDirective(ParseName(n)))
                                .Concat(imports._aliases.OrderBy(a => a.Key, StringComparer.Ordinal).Select(a => UsingDirective(NameEquals(a.Key), ParseName(a.Value))))
                                .ToArray();
        return (rewritten, attributes, directives);
    }

    public override SyntaxNode? VisitQualifiedName(QualifiedNameSyntax node)
    {
        if (Namespace(node.Left) is not { } ns || !Namespaces.Contains(ns)) return base.VisitQualifiedName(node);

        switch (node.Right)
        {
            case GenericNameSyntax generic:
                var key = (generic.Identifier.ValueText, generic.Arity);
                if (_generics.TryGetValue(key, out var existing) && existing != ns) return base.VisitQualifiedName(node);
                _generics[key] = ns;
                _imports.Add(ns);
                return Visit(generic)!.WithTriviaFrom(node);
            case IdentifierNameSyntax identifier:
                var name = identifier.Identifier.ValueText;
                var isAttribute = node.Parent is AttributeSyntax attribute && attribute.Name == node;
                var alias = isAttribute && !name.EndsWith("Attribute", StringComparison.Ordinal) ? name + "Attribute" : name;
                var target = "global::" + ns + "." + alias;
                if (_aliases.TryGetValue(alias, out var named) && named != target) return node;
                _aliases[alias] = target;
                var written = isAttribute ? alias[..^"Attribute".Length] : alias;
                return IdentifierName(written).WithTriviaFrom(node);
            default:
                return base.VisitQualifiedName(node);
        }
    }

    // Extensions.Method(receiver, rest) → receiver.Method(rest), importing the extension class's namespace.
    public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        if (node is not { Expression: MemberAccessExpressionSyntax { Expression: QualifiedNameSyntax type } access, ArgumentList.Arguments: [var first, ..] arguments }
         || type.Right is not IdentifierNameSyntax { Identifier.ValueText: var className }
         || !className.EndsWith("Extensions", StringComparison.Ordinal)
         || Namespace(type.Left) is not { } ns
         || !Namespaces.Contains(ns)
         || first.NameColon is { }
         || !first.RefKindKeyword.IsKind(SyntaxKind.None))
        {
            return base.VisitInvocationExpression(node);
        }

        _imports.Add(ns);
        var receiver = (ExpressionSyntax)Visit(first.Expression)!;
        if (receiver is not ( IdentifierNameSyntax or GenericNameSyntax or MemberAccessExpressionSyntax or InvocationExpressionSyntax or ElementAccessExpressionSyntax
                             or ThisExpressionSyntax or ParenthesizedExpressionSyntax or LiteralExpressionSyntax or ObjectCreationExpressionSyntax ))
        {
            receiver = ParenthesizedExpression(receiver);
        }

        var rest = SeparatedList(arguments.Skip(1).Select(a => (ArgumentSyntax)Visit(a)!));
        return InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, receiver, (SimpleNameSyntax)Visit(access.Name)!), ArgumentList(rest))
           .WithTriviaFrom(node);
    }

    // Leaves directives and namespace declarations as they are.
    public override SyntaxNode? VisitUsingDirective(UsingDirectiveSyntax node) => node;

    // The dotted namespace a name stands for, without global::; null for anything else.
    private static string? Namespace(NameSyntax name) => name switch
    {
        AliasQualifiedNameSyntax { Alias.Identifier.ValueText: "global", Name: IdentifierNameSyntax root } => root.Identifier.ValueText,
        QualifiedNameSyntax { Right: IdentifierNameSyntax right } qualified when Namespace(qualified.Left) is { } left => left + "." + right.Identifier.ValueText,
        _ => null,
    };
}
