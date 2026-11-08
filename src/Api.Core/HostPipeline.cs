using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

    /// <summary>The build property the base package's targets file sets to the mediator generators that run in this project.</summary>
    public const string MediatorGeneratorsProperty = "build_property.LaunchPadApiMediatorGenerators";

    /// <summary>
    ///     The analyzer file names of the mediator generators that run in this project, or <see langword="null" /> when the build didn't say
    ///     (no MSBuild, or the base package's targets weren't imported), in which case placement isn't checked.
    /// </summary>
    public static EquatableArray<string>? MediatorGenerators(Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptions options) =>
        options.TryGetValue(MediatorGeneratorsProperty, out var value)
            ? value.Split(['|'], StringSplitOptions.RemoveEmptyEntries).Select(v => v.Trim()).Where(v => v.Length > 0).ToEquatableArray()
            : null;

    /// <summary>Why a client host can't live in this project with its mediator (LPAD1012), or <see langword="null" /> when it can.</summary>
    public static string? PlacementProblem(ProfileModel profile, EquatableArray<string>? mediatorGenerators)
    {
        if (mediatorGenerators is not { } running || profile.MediatorGeneratorMarker is not { } marker) return null;
        var runsHere = running.Contains(marker);
        return profile.ClientPlacement switch
        {
            ClientPlacement.NotWithMediatorGenerator when runsHere =>
                $"this project runs {marker}, which can't see the generated handlers; move the host to a library that references the contracts but not {marker}, and reference that library from the application",
            ClientPlacement.WithMediatorGenerator when !runsHere =>
                $"the generated handlers extend the mediator that {marker} generates, so the host must be in a project that runs {marker}",
            ClientPlacement.Anywhere or ClientPlacement.NotWithMediatorGenerator or ClientPlacement.WithMediatorGenerator => null,
            _ => throw new ArgumentOutOfRangeException(nameof(profile)),
        };
    }

    /// <summary>
    ///     The marker the base package's analyzer looks for to know this transport's generator is referenced. Its <c>[Embedded]</c> attribute
    ///     comes from the base package's generator, which every transport package brings along.
    /// </summary>
    public static void EmitMarker(IncrementalGeneratorPostInitializationContext context, params HostKind[] kinds)
    {
        var markers = kinds.Select(
            kind => (MemberDeclarationSyntax)ApiSyntax.Class(kind + "Generator", ApiSyntax.Modifiers(SyntaxKind.InternalKeyword, SyntaxKind.StaticKeyword), [])
                                                     .AddAttributeLists(ApiSyntax.Embedded)
        );
        context.AddSource("Rocket.Surgery.LaunchPad.Api.Generated." + string.Join(".", kinds) + ".g.cs", ApiSyntax.Render("Rocket.Surgery.LaunchPad.Api.Generated", markers));
    }

    private static bool IsClass(SyntaxNode node, CancellationToken _) => node is ClassDeclarationSyntax;

    private static string HostName(GeneratorAttributeSyntaxContext context, CancellationToken _) => ( (INamedTypeSymbol)context.TargetSymbol ).MetadataName();
}
