using System.Net.Http.Json;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Rocket.Surgery.LaunchPad.Api.Generators;
using TUnit.Core.Interfaces;

namespace Api.Generators.Tests;

internal sealed class CollectibleTestAssemblyLoadContext() : AssemblyLoadContext(true), IDisposable
{
    public void Dispose() => Unload();

    protected override Assembly? Load(AssemblyName assemblyName) => null;
}

public enum Mediator
{
    MediatR,
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
        Mediator.MediatR => [typeof(MediatR.ISender).Assembly, typeof(MediatR.IRequest<>).Assembly],
        Mediator.Mediator => [typeof(global::Mediator.ISender).Assembly],
        Mediator.ChannelMediator => [typeof(ChannelMediator.IMediator).Assembly, typeof(ChannelMediator.IRequest<>).Assembly],
        Mediator.ImmediateHandlers => [typeof(Immediate.Handlers.Shared.IHandler<,>).Assembly],
        Mediator.Foundatio => [typeof(Foundatio.Mediator.IMediator).Assembly],
        Mediator.ZeroAlloc => [typeof(ZeroAlloc.Mediator.IRequest<>).Assembly],
        Mediator.Custom => [],
        _ => throw new ArgumentOutOfRangeException(nameof(mediator)),
    };

    /// <summary>Compiles a contracts project with the base package's attributes and returns it as a reference.</summary>
    protected async Task<GeneratorTestResults> CompileContracts(Mediator mediator, string source, params Assembly[] references)
    {
        var result = await GeneratorTestContextBuilder
                          .Create()
                          .WithLogger(Logger)
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
    ///     Snapshots the whole generator result: every added source, the generator, analyzer and compilation diagnostics. Only the .NET SDK's
    ///     default <c>NoWarn</c> (CS1701 and CS1702, assembly-version unification) is left out: a real build never shows it, and which references
    ///     raise it differs between target frameworks.
    /// </summary>
    protected static SettingsTask Verify(GeneratorTestResults result, [System.Runtime.CompilerServices.CallerFilePath] string sourceFile = "") =>
        Verifier.Verify(result with { FinalDiagnostics = result.FinalDiagnostics.RemoveAll(d => d.Id is "CS1701" or "CS1702") }, sourceFile: sourceFile);

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

    private GeneratorTestContextBuilder ServerBase() =>
        GeneratorTestContextBuilder
           .Create()
           .WithLogger(Logger)
           .WithAssemblyLoadContext(AssemblyLoadContext)
           .WithProjectName("Server")
           .AddPreprocessorSymbol("NET7_0_OR_GREATER")
           .AddSources(AttributesInHostSource ? [] : [AttributeSources])
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

    /// <summary>A client compilation running the gRPC client generator.</summary>
    protected GeneratorTestContextBuilder GrpcClient(Mediator mediator, GeneratorTestResults contracts) =>
        Client(mediator, contracts, withHttp: false)
           .WithGenerator<GrpcClientGenerator>()
           .AddReferences(GrpcAssemblies)
           .AddReferences(typeof(ProtoBuf.Grpc.ClientFactory.ServicesExtensions).Assembly, typeof(Grpc.Net.ClientFactory.GrpcClientFactoryOptions).Assembly);

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
                typeof(HotChocolate.Language.OperationType).Assembly
            );

    /// <summary>A client compilation running the GraphQL client generator.</summary>
    protected GeneratorTestContextBuilder GraphQLClient(Mediator mediator, GeneratorTestResults contracts) =>
        Client(mediator, contracts, withHttp: false).WithGenerator<GraphQLClientGenerator>();

    private static readonly Assembly[] GrpcAssemblies =
        [typeof(ProtoBuf.Grpc.CallContext).Assembly, typeof(Grpc.Core.CallOptions).Assembly, typeof(ProtoBuf.Serializer).Assembly];

    protected GeneratorTestContextBuilder Client(Mediator mediator, GeneratorTestResults contracts, bool withHttp = true) =>
        ( withHttp ? ClientBase().WithGenerator<HttpClientGenerator>() : ClientBase() )
       .AddCompilationReferences(contracts)
       .AddReferences(MediatorAssemblies(mediator));

    private GeneratorTestContextBuilder ClientBase() =>
        GeneratorTestContextBuilder
           .Create()
           .WithLogger(Logger)
           .WithAssemblyLoadContext(AssemblyLoadContext)
           .WithProjectName("Client")
           .AddPreprocessorSymbol("NET7_0_OR_GREATER")
           .AddSources(AttributesInHostSource ? [] : [AttributeSources])
           .WithAnalyzer<OperationHostAnalyzer>()
           .AddReferences(FrameworkReferences)
           .AddReferences(
                typeof(IHttpClientFactory).Assembly,
                typeof(HttpClientFactoryServiceCollectionExtensions).Assembly,
                typeof(IServiceCollection).Assembly,
                typeof(Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions).Assembly,
                typeof(JsonContent).Assembly,
                typeof(System.Text.Json.JsonSerializer).Assembly
            );
}
