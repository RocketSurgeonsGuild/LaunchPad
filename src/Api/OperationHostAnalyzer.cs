using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     Reports the host-level rules once per host, whichever transport packages are referenced: the host must be partial (static for Minimal API),
///     its transport must be inferable and have a generator, client hosts must not declare constructors, and host settings must apply.
/// </summary>
/// <remarks>
///     Analyzers see generator output, so "is the generator package referenced" is answered by the marker type each transport generator emits.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class OperationHostAnalyzer : DiagnosticAnalyzer
{
    internal const string GeneratorMarkerNamespace = "Rocket.Surgery.LaunchPad.Api.Generated";

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        ApiDiagnostics.HostMustBePartial, ApiDiagnostics.HostTransportUnknown, ApiDiagnostics.ClientHostConstructor, ApiDiagnostics.HostSettingNotApplicable,
    ];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(Analyze, SymbolKind.NamedType);
    }

    private static void Analyze(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol host) return;
        var entries = host.GetAttributes().Where(a => a.AttributeClass is { } c && c.Is(KnownNames.OperationAttribute)).ToArray();
        if (entries.Length == 0) return;

        var location = host.Locations.FirstOrDefault() ?? Location.None;
        var (kind, problem) = HostInference.Infer(host, context.Compilation);
        if (kind == HostKind.Unknown)
        {
            context.ReportDiagnostic(Diagnostic.Create(ApiDiagnostics.HostTransportUnknown, location, host.Name, problem));
            return;
        }

        if (context.Compilation.GetTypeByMetadataName(GeneratorMarkerNamespace + "." + kind + "Generator") is null)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    ApiDiagnostics.HostTransportUnknown,
                    location,
                    host.Name,
                    $"it is a {kind.Describe()} host, but the generator for that transport is not referenced (add the {PackageFor(kind)} package)"
                )
            );
        }

        // Only the declarations the user wrote: the generated part is always `static partial`, which would make any host look right.
        var declarations = host.DeclaringSyntaxReferences
                               .Where(r => !IsGenerated(r.SyntaxTree, context.CancellationToken))
                               .Select(r => r.GetSyntax(context.CancellationToken))
                               .OfType<TypeDeclarationSyntax>()
                               .ToArray();
        var isPartial = declarations.All(d => d.Modifiers.Any(SyntaxKind.PartialKeyword));
        var needsStatic = kind == HostKind.MinimalApi;
        var isStatic = declarations.Any(d => d.Modifiers.Any(SyntaxKind.StaticKeyword));
        if (!isPartial || needsStatic && !isStatic || host.ContainingType is { })
        {
            var requirement = host.ContainingType is { } ? "a top-level partial class" : needsStatic ? "static partial" : "partial";
            context.ReportDiagnostic(Diagnostic.Create(ApiDiagnostics.HostMustBePartial, location, ImmutableDictionary<string, string?>.Empty.Add("static", needsStatic ? "true" : "false"), host.Name, requirement));
        }

        // The client's own constructor is generated; only constructors the user wrote count.
        if (kind.IsClient() && host.InstanceConstructors.Any(c => !c.IsImplicitlyDeclared && c.DeclaringSyntaxReferences.Any(r => !IsGenerated(r.SyntaxTree, context.CancellationToken))))
        {
            context.ReportDiagnostic(Diagnostic.Create(ApiDiagnostics.ClientHostConstructor, location, host.Name));
        }

        var settings = host.GetAttributes().FirstOrDefault(a => a.AttributeClass.Is(KnownNames.OperationHostAttribute));
        if (settings is null) return;
        foreach (var setting in settings.NamedArguments.Select(n => n.Key))
        {
            var applies = setting switch
            {
                "ClientName" or "Handlers" or "JsonOptions" => kind.IsClient(),
                "RoutePrefix" => kind is HostKind.MinimalApi or HostKind.Controller or HostKind.HttpClient,
                _ => true,
            };
            if (!applies)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(ApiDiagnostics.HostSettingNotApplicable, settings.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? location, setting, host.Name, kind.Describe())
                );
            }
        }
    }

    private static bool IsGenerated(SyntaxTree tree, CancellationToken cancellationToken) =>
        tree.FilePath.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase)
     || tree.GetRoot(cancellationToken).GetLeadingTrivia().Any(t => t.ToString().Contains("<auto-generated"));

    private static string PackageFor(HostKind kind) => kind switch
    {
        HostKind.MinimalApi => "Rocket.Surgery.LaunchPad.Api.MinimalApi",
        HostKind.Controller => "Rocket.Surgery.LaunchPad.Api.Controllers",
        HostKind.GrpcService => "Rocket.Surgery.LaunchPad.Api.Grpc",
        HostKind.GraphQLQuery or HostKind.GraphQLMutation => "Rocket.Surgery.LaunchPad.Api.GraphQL",
        HostKind.HttpClient => "Rocket.Surgery.LaunchPad.Api.Client.Http",
        HostKind.GrpcClient => "Rocket.Surgery.LaunchPad.Api.Client.Grpc",
        HostKind.GraphQLClient => "Rocket.Surgery.LaunchPad.Api.Client.GraphQL",
        HostKind.Unknown => "transport",
        _ => throw new ArgumentOutOfRangeException(nameof(kind)),
    };
}
