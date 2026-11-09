using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     The incremental pipeline every transport generator shares.
/// </summary>
/// <remarks>
///     <para>
///         Hosts are found with ForAttributeWithMetadataName, which only yields their metadata names, so that step is cached per syntax tree.
///     </para>
///     <para>
///         The hosts are then read against the current compilation. A transform on the attribute alone would miss an edit to a listed request
///         type in another file. Only the listed types are looked up, so the step is cheap, and its output is equatable: emission re-runs only
///         when an operation actually changed.
///     </para>
/// </remarks>
internal static class HostPipeline
{
    public static IncrementalValueProvider<EquatableArray<HostModel>> Hosts(IncrementalGeneratorInitializationContext context, params HostKind[] kinds)
    {
        var names = context.SyntaxProvider.ForAttributeWithMetadataName(KnownNames.OperationAttribute, IsClass, HostName)
                           .Collect()
                           .Select(static (hosts, _) => hosts.Distinct().OrderBy(n => n, StringComparer.Ordinal).ToEquatableArray())
                           .WithTrackingName("HostNames");

        return names.Combine(context.CompilationProvider)
                    .Combine(Documentation.Provider(context))
                    .Select(
                         (pair, cancellationToken) =>
                         {
                             var ((hostNames, compilation), documentation) = pair;
                             if (hostNames.IsEmpty) return EquatableArray<HostModel>.Empty;
                             var profile = ProfileResolver.Resolve(compilation);
                             var referenced = documentation.GroupBy(e => e.Id, StringComparer.Ordinal).ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal);
                             return hostNames
                                   .Select(name => compilation.GetTypeByMetadataName(name))
                                   .OfType<INamedTypeSymbol>()
                                   .Where(host => kinds.Contains(HostInference.Infer(host, compilation).Kind))
                                   .Select(host => HostReader.Read(host, compilation, profile, referenced, cancellationToken))
                                   .ToArray()
                                   .ResolveCreatedLocations()
                                   .ToEquatableArray();
                         }
                     )
                    .WithTrackingName("Hosts");
    }


    private static bool IsClass(SyntaxNode node, CancellationToken _) => node is ClassDeclarationSyntax;

    private static string HostName(GeneratorAttributeSyntaxContext context, CancellationToken _) => ( (INamedTypeSymbol)context.TargetSymbol ).MetadataName();
}
