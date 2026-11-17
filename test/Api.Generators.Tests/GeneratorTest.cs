using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Rocket.Surgery.LaunchPad.Analyzers.Api;
using Rocket.Surgery.LaunchPad.Analyzers.Api.Controllers;
using Rocket.Surgery.LaunchPad.Analyzers.Api.GraphQL;
using Rocket.Surgery.LaunchPad.Analyzers.Api.Grpc;
using Rocket.Surgery.LaunchPad.Analyzers.Api.MinimalApi;
using TUnit.Core.Interfaces;

namespace Api.Generators.Tests;

internal sealed class CollectibleTestAssemblyLoadContext() : AssemblyLoadContext(true), IDisposable
{
    public void Dispose() => Unload();

    protected override Assembly? Load(AssemblyName assemblyName) => null;
}

public enum Mediator
{
    Mediator,
    ChannelMediator,
    ImmediateHandlers,
    Foundatio,
    ZeroAlloc,

    /// <summary>No built-in mediator: the compilation describes its own with <c>[assembly: MediatorProfile]</c>.</summary>
    Custom,
}

/// <summary>
///     Builds test compilations the way real projects are laid out: the contracts are compiled first, as their own assembly with the base
///     package's attributes, and the server or client compilation references it.
/// </summary>
public abstract class GeneratorTest() : LoggerTest<TestRecord>(TestRecord.Create()), IAsyncInitializer
{
    public AssemblyLoadContext AssemblyLoadContext { get; } = new CollectibleTestAssemblyLoadContext();

    /// <summary>
    ///     The test appends <see cref="AttributeSources" /> to its own host source, so the compilation has one document. Code fix tests need it:
    ///     the harness doesn't keep several documents paired between the compilations it compares, and reports a swap as an edit.
    /// </summary>
    protected bool AttributesInHostSource { get; set; }

    public virtual Task InitializeAsync()
    {
        if (AssemblyLoadContext is IDisposable disposable) Disposables.Add(disposable);
        return Task.CompletedTask;
    }

    // A real project compiles against the whole shared framework; the harness's defaults are minimal, so hand it the same set.
    protected static readonly MetadataReference[] FrameworkReferences =
        [.. ( (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")! )
       .Split(Path.PathSeparator)
       .Where(path => Path.GetFileName(path) is var name && ( name.StartsWith("System.", StringComparison.Ordinal) || name.StartsWith("Microsoft.AspNetCore.", StringComparison.Ordinal) || name.StartsWith("Microsoft.Extensions.", StringComparison.Ordinal) || name is "netstandard.dll" or "mscorlib.dll" ))
       .Where(path => path.Contains("Microsoft.NETCore.App", StringComparison.Ordinal) || path.Contains("Microsoft.AspNetCore.App", StringComparison.Ordinal))
       .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))];

    // The harness runs every generator in its own driver, but in a real build the transport generators see the base package's post-init
    // output (the attributes). Emit that output once and hand it to server and client compilations as one ordinary source: the harness
    // doesn't keep the order of several sources between the compilations it builds for a code fix, and would report the swap as an edit.
    // The embedded attribute's own file goes last, after the files with using directives.
    protected static readonly string AttributeSources = string.Join(
        "\n",
        CSharpGeneratorDriver
           .Create(new AttributesGenerator())
           .RunGenerators(CSharpCompilation.Create("Attributes", references: FrameworkReferences))
           .GetRunResult()
           .GeneratedTrees
           .OrderBy(tree => tree.FilePath.EndsWith("Microsoft.CodeAnalysis.EmbeddedAttribute.cs", StringComparison.Ordinal))
           .ThenBy(tree => tree.FilePath, StringComparer.Ordinal)
           .Select(tree => tree.GetText().ToString())
    );

    protected static Assembly[] MediatorAssemblies(Mediator mediator) => mediator switch
    {
        Mediator.Mediator => [typeof(global::Mediator.ISender).Assembly],
        Mediator.ChannelMediator => [typeof(ChannelMediator.IMediator).Assembly, typeof(ChannelMediator.IRequest<>).Assembly],
        Mediator.ImmediateHandlers => [typeof(Immediate.Handlers.Shared.IHandler<,>).Assembly],
        Mediator.Foundatio => [typeof(Foundatio.Mediator.IMediator).Assembly],
        Mediator.ZeroAlloc => [typeof(ZeroAlloc.Mediator.IRequest<>).Assembly],
        Mediator.Custom => [],
        _ => throw new ArgumentOutOfRangeException(nameof(mediator)),
    };

    /// <summary>Compiles a contracts project with the base package's attributes and returns it as a reference.</summary>
    protected Task<GeneratorTestResults> CompileContracts(Mediator mediator, string source, params Assembly[] references) =>
        CompileContracts(mediator, source, LanguageVersion.Preview, references);

    /// <summary>Compiles a contracts project with the given language version (a native <c>union</c> declaration needs C# 15).</summary>
    protected async Task<GeneratorTestResults> CompileContracts(Mediator mediator, string source, LanguageVersion languageVersion, params Assembly[] references)
    {
        var result = await GeneratorTestContextBuilder
                          .Create()
                          .WithLogger(Logger)
                          .WithLanguageVersion(languageVersion)
                          .WithAssemblyLoadContext(AssemblyLoadContext)
                          .WithProjectName("Contracts")
                          .WithGenerator<AttributesGenerator>()
                          .AddReferences(MediatorAssemblies(mediator))
                          .AddReferences(references)
                          .AddSources(source)
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        AssertCompiles(result);
        return result;
    }

    /// <summary>
    ///     Snapshots the whole generator result: every added source, the generator, analyzer and compilation diagnostics. Left out: the .NET
    ///     SDK's default <c>NoWarn</c> (CS1701 and CS1702, assembly-version unification), which a real build never shows and which references
    ///     raise differs between target frameworks, and the constant helper sources, identical in every snapshot.
    /// </summary>
    protected static SettingsTask Verify(GeneratorTestResults result, [System.Runtime.CompilerServices.CallerFilePath] string sourceFile = "") =>
        Verifier
           .Verify(result with { FinalDiagnostics = result.FinalDiagnostics.RemoveAll(d => d.Id is "CS1701" or "CS1702") }, sourceFile: sourceFile);


    /// <summary>
    ///     Snapshots only the diagnostics, for tests about a diagnostic: everything the generators, the analyzer and the final compilation
    ///     reported, Info and up, as one list without repeats, plus each code fix the test ran: its title and the fixed document.
    /// </summary>
    protected async Task VerifyDiagnostics(GeneratorTestResults result, Func<SettingsTask, SettingsTask>? settings = null, [System.Runtime.CompilerServices.CallerFilePath] string sourceFile = "")
    {
        var diagnostics = result.Results.Values.SelectMany(r => r.Diagnostics)
                                .Concat(result.AnalyzerResults.Values.SelectMany(r => r.Diagnostics))
                                .Concat(result.FinalDiagnostics)
                                .Where(d => d.Severity >= DiagnosticSeverity.Info && d.Id is not ( "CS1701" or "CS1702" ))
                                .GroupBy(d => (d.Id, Location: d.Location.ToString(), Message: d.GetMessage(System.Globalization.CultureInfo.InvariantCulture)))
                                .Select(g => g.First())
                                .OrderBy(d => d.Location.SourceTree?.FilePath, StringComparer.Ordinal)
                                .ThenBy(d => d.Location.SourceSpan.Start)
                                .ThenBy(d => d.Id, StringComparer.Ordinal)
                                .ToArray();
        var fixes = new List<object>();
        // The harness can report one diagnostic more than once; each fix is applied to the workspace the previous one left, so only the first
        // of a diagnostic shows what the fix does.
        var fixedDiagnostics = new HashSet<(string, string, string)>();
        foreach (var resolved in result.CodeFixResults.Values.SelectMany(r => r.ResolvedFixes))
        {
            if (resolved.CodeActions.IsEmpty || !fixedDiagnostics.Add((resolved.Diagnostic.Id, resolved.Diagnostic.Location.ToString(), resolved.Diagnostic.GetMessage(System.Globalization.CultureInfo.InvariantCulture)))) continue;
            foreach (var action in resolved.CodeActions)
            {
                var others = await OtherDocuments(action.Changes, action.TargetDocument.Id);
                if (others.Count > 0)
                {
                    fixes.Add(new { action.CodeAction.Title, Documents = others });
                    continue;
                }

                // The document as the fix leaves it: TargetDocument is the one it started from.
                var fixedDocument = action.Changes.NewProject.GetDocument(action.TargetDocument.Id) ?? action.TargetDocument;
                var text = ( await fixedDocument.GetTextAsync(TestContext.CancellationToken) ).ToString();
                // The attribute declarations appended to the host source (AttributesInHostSource) aren't part of the fix.
                if (AttributesInHostSource && text.IndexOf(AttributeSources, StringComparison.Ordinal) is >= 0 and var start) text = text[..start].TrimEnd();
                fixes.Add(new { action.CodeAction.Title, Document = text });
            }
        }

        object target = fixes.Count == 0 ? new { Diagnostics = diagnostics } : new { Diagnostics = diagnostics, CodeFixes = fixes };
        var task = Verifier.Verify(target, sourceFile: sourceFile);
        await ( settings?.Invoke(task) ?? task );
    }

    /// <summary>
    ///     Snapshots what <typeparamref name="TProvider" />'s fix all does in <paramref name="scope" /> with every diagnostic the harness offered
    ///     the provider: the action's title and each document it adds or changes. The harness runs only single fixes, so this builds the
    ///     <see cref="FixAllContext" /> an IDE would.
    /// </summary>
    protected async Task VerifyFixAll<TProvider>(GeneratorTestResults result, FixAllScope scope, Func<SettingsTask, SettingsTask>? settings = null, [System.Runtime.CompilerServices.CallerFilePath] string sourceFile = "")
        where TProvider : CodeFixProvider, new()
    {
        var provider = new TProvider();
        var offered = result.CodeFixResults[typeof(TProvider)].ResolvedFixes.Where(r => !r.CodeActions.IsEmpty).ToArray();
        if (offered.Length == 0) throw new InvalidOperationException($"{typeof(TProvider).Name} was offered no diagnostic to fix.");
        var document = offered[0].Document;
        var context = new FixAllContext(
            document,
            provider,
            scope,
            offered[0].CodeActions[0].CodeAction.EquivalenceKey,
            provider.FixableDiagnosticIds,
            new OfferedDiagnostics([.. offered.Select(r => r.Diagnostic)]),
            TestContext.CancellationToken
        );
        var action = await provider.GetFixAllProvider()!.GetFixAsync(context) ?? throw new InvalidOperationException("The fix all offered no action.");
        var operations = await action.GetOperationsAsync(TestContext.CancellationToken);
        var changed = operations.OfType<ApplyChangesOperation>().Single().ChangedSolution;
        var documents = new List<object>();
        foreach (var changes in changed.GetChanges(document.Project.Solution).GetProjectChanges())
        {
            documents.AddRange(await OtherDocuments(changes, null));
        }

        var task = Verifier.Verify(new { action.Title, Documents = documents }, sourceFile: sourceFile);
        await ( settings?.Invoke(task) ?? task );
    }

    /// <summary>The diagnostics a fix all sees: the ones the harness offered the provider, which the generators report by file path.</summary>
    private sealed class OfferedDiagnostics(ImmutableArray<Diagnostic> diagnostics) : FixAllContext.DiagnosticProvider
    {
        public override Task<IEnumerable<Diagnostic>> GetDocumentDiagnosticsAsync(Document document, CancellationToken cancellationToken) =>
            Task.FromResult(diagnostics.Where(d => d.Location.GetLineSpan().Path == ( document.FilePath ?? document.Name )));

        public override Task<IEnumerable<Diagnostic>> GetProjectDiagnosticsAsync(Project project, CancellationToken cancellationToken) =>
            Task.FromResult(diagnostics.Where(d => d.Location == Location.None));

        public override Task<IEnumerable<Diagnostic>> GetAllDiagnosticsAsync(Project project, CancellationToken cancellationToken) =>
            Task.FromResult<IEnumerable<Diagnostic>>(diagnostics);
    }

    /// <summary>
    ///     The documents a fix adds, or whose text it changes, other than the one it started from (LPAD1030's fix writes onto the JSON context,
    ///     or creates one). Compared by text: the harness reports documents it re-created as changed.
    /// </summary>
    private async Task<List<object>> OtherDocuments(ProjectChanges changes, DocumentId? target)
    {
        var documents = new List<object>();
        foreach (var document in changes.NewProject.Documents)
        {
            if (document.Id == target) continue;
            var text = ( await document.GetTextAsync(TestContext.CancellationToken) ).ToString();
            if (changes.OldProject.GetDocument(document.Id) is { } old && ( await old.GetTextAsync(TestContext.CancellationToken) ).ToString() == text) continue;
            documents.Add(new { document.Name, Text = text });
        }

        return documents;
    }

    /// <summary>Fails with the final compilation's errors listed, so a broken emitter shows what it broke.</summary>
    protected static void AssertCompiles(GeneratorTestResults result)
    {
        var errors = result.FinalDiagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => d.ToString()).ToArray();
        if (errors.Length > 0) throw new InvalidOperationException("The generated code does not compile:\n" + string.Join("\n", errors));
    }


    protected GeneratorTestContextBuilder Server(Mediator mediator, GeneratorTestResults contracts, bool withMinimalApi = true) =>
        ( withMinimalApi ? ServerBase().WithGenerator<MinimalApiGenerator>() : ServerBase() )
       .AddCompilationReferences(contracts)
       .AddReferences(MediatorAssemblies(mediator));

    /// <summary>
    ///     Whether the server compilations report LPAD1030. The test contracts declare no JSON context, so every Minimal API operation would
    ///     report it; only the tests of the check itself (<see cref="JsonMetadataTests" />) keep it on, the rest turn it off as an application can.
    /// </summary>
    protected virtual bool ReportsJsonMetadata => false;

    private GeneratorTestContextBuilder ServerBase() =>
        ( ReportsJsonMetadata
            ? GeneratorTestContextBuilder.Create()
            : GeneratorTestContextBuilder.Create().AddGlobalOption("build_property.LaunchPadApiRequireJsonContext", "false") )
           .WithLogger(Logger)
           .WithAssemblyLoadContext(AssemblyLoadContext)
           .WithProjectName("Server")
           .AddPreprocessorSymbol("NET7_0_OR_GREATER")
           .AddSources(AttributesInHostSource ? [] : [AttributeSources])
           .IgnoreHelperSources()
           .WithAnalyzer<OperationHostAnalyzer>()
           .AddReferences(FrameworkReferences)
           .AddReferences(
                typeof(WebApplication).Assembly,
                typeof(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder).Assembly,
                typeof(EndpointRouteBuilderExtensions).Assembly,
                typeof(RouteHandlerBuilder).Assembly,
                typeof(TypedResults).Assembly,
                typeof(IResult).Assembly,
                typeof(HttpContext).Assembly,
                typeof(Microsoft.AspNetCore.Mvc.FromRouteAttribute).Assembly,
                typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute).Assembly,
                typeof(AuthorizationEndpointConventionBuilderExtensions).Assembly,
                typeof(OpenApiRouteHandlerBuilderExtensions).Assembly,
                typeof(Microsoft.AspNetCore.Http.Json.JsonOptions).Assembly,
                typeof(Microsoft.Extensions.Options.IOptions<>).Assembly,
                typeof(ActivatorUtilities).Assembly
            );

    /// <summary>A server compilation running the controller generator instead of the Minimal API one.</summary>
    protected GeneratorTestContextBuilder Controllers(Mediator mediator, GeneratorTestResults contracts) =>
        Server(mediator, contracts, withMinimalApi: false)
           .WithGenerator<ControllerGenerator>()
           .AddReferences(typeof(Microsoft.AspNetCore.Mvc.ControllerBase).Assembly, typeof(Microsoft.AspNetCore.Mvc.JsonOptions).Assembly);

    /// <summary>A server compilation running the gRPC service generator.</summary>
    protected GeneratorTestContextBuilder GrpcServer(Mediator mediator, GeneratorTestResults contracts) =>
        Server(mediator, contracts, withMinimalApi: false)
           .WithGenerator<GrpcServiceGenerator>()
           .AddReferences(GrpcAssemblies)
           .AddReferences(typeof(Grpc.AspNetCore.Server.GrpcServiceOptions).Assembly);


    /// <summary>A server compilation running the GraphQL mutation generator.</summary>
    protected GeneratorTestContextBuilder GraphQLServer(Mediator mediator, GeneratorTestResults contracts) =>
        Server(mediator, contracts, withMinimalApi: false)
           .WithGenerator<GraphQLMutationGenerator>()
           .AddReferences(
                typeof(HotChocolate.Types.ExtendObjectTypeAttribute).Assembly,
                typeof(HotChocolate.GraphQLNameAttribute).Assembly,
                typeof(HotChocolate.Resolvers.IResolverContext).Assembly,
                typeof(HotChocolate.Execution.Configuration.IRequestExecutorBuilder).Assembly,
                typeof(SchemaRequestExecutorBuilderExtensions).Assembly,
                typeof(HotChocolate.Authorization.AuthorizeAttribute).Assembly,
                typeof(HotChocolate.Types.IInputObjectTypeDescriptor<>).Assembly,
                typeof(HotChocolate.Language.OperationType).Assembly,
                // HotChocolate 16 moved GraphQLException, IError and ErrorBuilder to HotChocolate.Execution.Abstractions, and the
                // resolver context carries the IFeatureProvider of HotChocolate.Features
                typeof(HotChocolate.GraphQLException).Assembly,
                typeof(HotChocolate.Features.IFeatureProvider).Assembly
            );


    /// <summary>
    ///     A server compilation running the proto-first gRPC generator, with Grpc.AspNetCore and, unless <paramref name="mapping" /> is false,
    ///     ZeroAlloc.Mapping (whose generator doesn't run here: the tests write out the methods it would generate).
    /// </summary>
    protected GeneratorTestContextBuilder ProtoGrpcServer(Mediator mediator, GeneratorTestResults contracts, bool mapping = true) =>
        Server(mediator, contracts, withMinimalApi: false)
           .WithGenerator<GrpcProtoServiceGenerator>()
           .AddReferences(
                typeof(Grpc.Core.ServerCallContext).Assembly,
                typeof(Google.Protobuf.IMessage).Assembly,
                typeof(Grpc.AspNetCore.Server.GrpcServiceOptions).Assembly,
                typeof(GrpcEndpointRouteBuilderExtensions).Assembly
            )
           .AddReferences(mapping ? [typeof(ZeroAlloc.Mapping.MapAttribute<,>).Assembly] : []);

    private static readonly Assembly[] GrpcAssemblies =
        [
            typeof(ProtoBuf.Grpc.CallContext).Assembly, typeof(Grpc.Core.CallOptions).Assembly, typeof(ProtoBuf.Serializer).Assembly, typeof(ProtoBuf.Meta.TypeModel).Assembly,
            // GetProto(): the service's .proto through protobuf-net.Grpc.Reflection.
            typeof(ProtoBuf.Grpc.Reflection.SchemaGenerator).Assembly,
        ];

}

internal static class GeneratorTestContextBuilderExtensions
{
    /// <summary>Leaves the constant helper sources out of the snapshots: they are the same text in every test with a host of their transport.</summary>
    public static GeneratorTestContextBuilder IgnoreHelperSources(this GeneratorTestContextBuilder builder) =>
        builder
           .IgnoreOutputFile("Rocket.Surgery.LaunchPad.Api.Generated.ServerHelpers.g.cs")
           .IgnoreOutputFile("Rocket.Surgery.LaunchPad.Api.Generated.ControllerHelpers.g.cs")
           .IgnoreOutputFile("Rocket.Surgery.LaunchPad.Api.Generated.GrpcHelpers.g.cs")
           .IgnoreOutputFile("Rocket.Surgery.LaunchPad.Api.Generated.GrpcProtoHelpers.g.cs")
           .IgnoreOutputFile("Rocket.Surgery.LaunchPad.Api.Generated.GraphQLHelpers.g.cs")
           .IgnoreOutputFile("Rocket.Surgery.LaunchPad.Api.Generated.MinimalApiOpenApi.g.cs")
           .IgnoreOutputFile("Rocket.Surgery.LaunchPad.Api.Generated.ControllerOpenApi.g.cs");
}
