using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Api.Generators.Tests;

/// <summary>The kinds of operation host, one per transport and side.</summary>
public enum HostCase
{
    MinimalApi,
    Controller,
    GrpcService,
    GraphQLMutation,
    HttpClient,
    GrpcClient,
    GraphQLClient,
}

/// <summary>
///     Every diagnostic, on every kind of host it can be reported for, with every mediator profile: each case compiles a contracts project and a
///     host built to raise exactly that diagnostic, and snapshots the reported diagnostics and the generated code into
///     <c>snapshots/diagnostics/{id}.{host}.{mediator}</c>.
/// </summary>
public class DiagnosticMatrixTests() : GeneratorTest()
{
    private static readonly Mediator[] BuiltIn =
        [Mediator.MediatR, Mediator.Mediator, Mediator.ChannelMediator, Mediator.ImmediateHandlers, Mediator.Foundatio, Mediator.ZeroAlloc];

    private static readonly HostCase[] AllHosts = Enum.GetValues<HostCase>();
    private static readonly HostCase[] Http = [HostCase.MinimalApi, HostCase.Controller, HostCase.HttpClient];
    private static readonly HostCase[] HttpServers = [HostCase.MinimalApi, HostCase.Controller];
    private static readonly HostCase[] Clients = [HostCase.HttpClient, HostCase.GrpcClient, HostCase.GraphQLClient];
    private static readonly HostCase[] Grpc = [HostCase.GrpcService, HostCase.GrpcClient];
    private static readonly HostCase[] GraphQL = [HostCase.GraphQLMutation, HostCase.GraphQLClient];

    /// <summary>The hosts and profiles each diagnostic can be reported for.</summary>
    private static readonly (string Id, HostCase[] Hosts, Mediator[] Mediators)[] Applicability =
    [
        ("LPAD1001", AllHosts, BuiltIn),
        ("LPAD1002", AllHosts, BuiltIn),
        ("LPAD1003", Http, BuiltIn),
        ("LPAD1004", Http, BuiltIn),
        ("LPAD1005", Http, BuiltIn),
        ("LPAD1006", Http, BuiltIn),
        ("LPAD1007", Http, BuiltIn),
        ("LPAD1008", Http, BuiltIn),
        ("LPAD1009", [HostCase.GraphQLMutation], BuiltIn),
        ("LPAD1010", AllHosts, BuiltIn),
        ("LPAD1011", Clients, [Mediator.Custom]),
        ("LPAD1012", Clients, [Mediator.Mediator, Mediator.Foundatio, Mediator.ZeroAlloc]),
        ("LPAD1013", Clients, BuiltIn),
        ("LPAD1014", Grpc, BuiltIn),
        ("LPAD1015", GraphQL, BuiltIn),
        ("LPAD1016", [HostCase.GraphQLClient], BuiltIn),
        ("LPAD1017", [HostCase.GrpcService, HostCase.GrpcClient, HostCase.GraphQLMutation], BuiltIn),
        ("LPAD1018", Clients, BuiltIn),
        ("LPAD1019", [HostCase.GraphQLMutation], BuiltIn),
        ("LPAD1020", AllHosts, BuiltIn),
        ("LPAD1021", [.. Grpc, .. GraphQL], BuiltIn),
        ("LPAD1022", Clients, BuiltIn),
        ("LPAD1023", HttpServers, BuiltIn),
        ("LPAD1024", HttpServers, BuiltIn),
    ];

    public static IEnumerable<(string Id, HostCase Host, Mediator Mediator)> Cases() =>
        from row in Applicability from host in row.Hosts from mediator in row.Mediators select (row.Id, host, mediator);

    [Test]
    [MethodDataSource(nameof(Cases))]
    public async Task Should_Report(string id, HostCase host, Mediator mediator)
    {
        var fixture = Fixture(id, host, mediator);
        // The code fix case needs a single document (see AttributesInHostSource).
        AttributesInHostSource = id == "LPAD1010";
        var contracts = await CompileContracts(mediator, MatrixContracts.For(mediator, fixture.DataContracts, fixture.HandlerFor), typeof(System.Runtime.Serialization.DataContractAttribute).Assembly);
        var builder = Builder(fixture.Builder ?? host, mediator, contracts);
        if (fixture.Configure is { } configure) builder = configure(builder);
        var result = await builder.AddSources(Source(fixture.Host, mediator) + ( AttributesInHostSource ? "\n" + AttributeSources : "" )).Build().GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseDirectory(Path.Combine("snapshots", "diagnostics")).UseFileName($"{id}.{host}.{mediator}");
    }

    /// <summary>What a case declares: the host source, how the contracts are written, and how the compilation is set up.</summary>
    private sealed record Case(
        string Host,
        bool DataContracts = true,
        string? HandlerFor = null,
        Func<GeneratorTestContextBuilder, GeneratorTestContextBuilder>? Configure = null,
        HostCase? Builder = null
    );

    private Case Fixture(string id, HostCase host, Mediator mediator)
    {
        var op = Operation(host);
        return id switch
        {
            // The request matches no request kind of the profile and has no Response.
            "LPAD1001" => new($"[Operation<OrphanRocket.Request>] {Declare(host)}"),
            // Two built-in profiles match: the compilation references a second mediator.
            "LPAD1002" => new(
                $"[Operation<{op}.Request>] {Declare(host)}",
                Configure: builder => builder.AddReferences(MediatorAssemblies(mediator == Mediator.MediatR ? Mediator.Mediator : Mediator.MediatR))
            ),
            "LPAD1003" => new($"[Operation<GetRocket.Request>(Route = \"{{rocketId}}\")] {Declare(host)}"),
            "LPAD1004" => new($"[Operation<LocateRocket.Request>(Route = \"locate\")] {Declare(host)}"),
            "LPAD1005" => new($"[Operation<SearchRockets.Request>] {Declare(host)}"),
            "LPAD1006" => new($"[Operation<GetRocketSnapshot.Request>] {Declare(host)}"),
            "LPAD1007" => new($"[Operation<GetRocketByCode.Request>] {Declare(host)}"),
            "LPAD1008" => new($"[Operation<GetRocket.Request>] [Operation<FindRocket.Request>] {Declare(host)}"),
            "LPAD1009" => new($"[Operation<CreateRocket.Request>] {Declare(host)} [Operation<CreateRocket.Request>] {Declare(host, "Fleet")}"),
            // With the code fix, so the snapshot shows the host it makes partial (and static, for Minimal API).
            "LPAD1010" => new(
                $"[Operation<{op}.Request>] {Declare(host, partial: false)}",
                Configure: builder => builder.WithCodeFix<Rocket.Surgery.LaunchPad.Api.Generators.MakeHostPartialCodeFix>()
            ),
            // A custom profile without a handler interface: every built-in profile has one for each request kind.
            "LPAD1011" => new($"[Operation<{op}.Request>] {Declare(host)}"),
            "LPAD1012" => new(
                $"[Operation<{op}.Request>] {Declare(host)}",
                Configure: builder => builder.AddGlobalOption("build_property.LaunchPadApiMediatorGenerators", WrongProjectGenerators(mediator))
            ),
            "LPAD1013" => new($"[Operation<{op}.Request>] {Declare(host)}", HandlerFor: op),
            "LPAD1014" => new($"[Operation<{op}.Request>] {Declare(host)}", DataContracts: false),
            "LPAD1015" => new($"[Operation<ListRockets.Request>(Name = \"CreateRockets\")] {Declare(host)}"),
            // A query on a GraphQL client: every built-in profile dispatches every request kind on the other transports.
            "LPAD1016" => new($"[Operation<GetRocket.Request>] {Declare(host)}"),
            // The host's transport library isn't referenced: a gRPC or GraphQL host in a project without protobuf-net.Grpc or HotChocolate. The
            // ASP.NET Core and Microsoft.Extensions.Http libraries of the other transports are part of every test compilation.
            "LPAD1017" => new($"[Operation<{op}.Request>] {Declare(host)}", Builder: IsClient(host) ? HostCase.MinimalApi : HostCase.HttpClient),
            "LPAD1018" => new($"[Operation<{op}.Request>] {Declare(host)} [Operation<{op}.Request>] {Declare(host, "Fleet")}"),
            "LPAD1019" => new($"[Operation<GetRocket.Request>] {Declare(host)}"),
            "LPAD1020" => new($"[Operation<{op}.Request>(Input = typeof(RocketModel))] {Declare(host)}"),
            "LPAD1021" => new($"[OperationHost(RoutePrefix = \"fleet\")] [Operation<{op}.Request>] {Declare(host)}"),
            "LPAD1022" => new($"[Operation<{op}.Request>] {Declare(host, body: $"public {Name(host)}() {{ }}")}"),
            "LPAD1023" => new($"[Operation<CreateRocket.Request>] {Declare(host)}"),
            "LPAD1024" => new($"[OperationHost(Version = \"1.0\")] [Operation<GetRocket.Request>] {Declare(host)}"),
            _ => throw new ArgumentOutOfRangeException(nameof(id), id, null),
        };
    }

    // GraphQL hosts only carry mutations.
    private static string Operation(HostCase host) => host is HostCase.GraphQLMutation or HostCase.GraphQLClient ? "CreateRocket" : "GetRocket";

    private static bool IsClient(HostCase host) => host is HostCase.HttpClient or HostCase.GrpcClient or HostCase.GraphQLClient;

    private static string Name(HostCase host, string noun = "Rockets") => noun + host switch
    {
        HostCase.MinimalApi => "Endpoints",
        HostCase.Controller => "Controller",
        HostCase.GrpcService => "Service",
        HostCase.GraphQLMutation => "Mutations",
        HostCase.HttpClient => "Client",
        HostCase.GrpcClient => "GrpcClient",
        HostCase.GraphQLClient => "GraphQLClient",
        _ => throw new ArgumentOutOfRangeException(nameof(host)),
    };

    private static string Declare(HostCase host, string noun = "Rockets", bool partial = true, string? body = null)
    {
        var modifiers = (host, partial) switch
        {
            (HostCase.MinimalApi, true) => "public static partial class",
            (HostCase.MinimalApi, false) => "public partial class",
            (_, true) => "public partial class",
            (_, false) => "public class",
        };
        return $"{modifiers} {Name(host, noun)}{( body is null ? ";" : $" {{ {body} }}" )}";
    }

    // The build property for a project that runs the wrong mediator generators for the profile's client placement. Only the source-generated
    // mediators constrain where a client host lives; MediatR, ChannelMediator and Immediate.Handlers clients can live anywhere.
    private static string WrongProjectGenerators(Mediator mediator) => mediator switch
    {
        Mediator.Mediator => "|Mediator.SourceGenerator|",
        Mediator.Foundatio => "|Foundatio.Mediator|",
        Mediator.ZeroAlloc => "|",
        Mediator.MediatR or Mediator.ChannelMediator or Mediator.ImmediateHandlers or Mediator.Custom => throw new ArgumentOutOfRangeException(nameof(mediator)),
        _ => throw new ArgumentOutOfRangeException(nameof(mediator)),
    };

    private const string CustomProfile = """
        [assembly: MediatorProfile(SenderType = typeof(IMyDispatcher), SendMethod = "Dispatch", StreamMethod = "Stream", Name = "MyMediator")]
        [assembly: MediatorRequest(typeof(IMyRequest<>))]
        [assembly: MediatorRequest(typeof(IMyStream<>), Kind = RequestKind.Stream)]
        """;

    private static string Source(string body, Mediator mediator) => $$"""
        #nullable enable
        using Rocket.Surgery.LaunchPad.Api;
        using Sample.Contracts.Rockets;

        {{( mediator == Mediator.Custom ? CustomProfile : "" )}}

        namespace Sample.Host
        {
            {{body}}
        }
        """;

    private GeneratorTestContextBuilder Builder(HostCase host, Mediator mediator, GeneratorTestResults contracts) => host switch
    {
        HostCase.MinimalApi => Server(mediator, contracts),
        HostCase.Controller => Controllers(mediator, contracts),
        HostCase.GrpcService => GrpcServer(mediator, contracts),
        HostCase.GraphQLMutation => GraphQLServer(mediator, contracts),
        HostCase.HttpClient => Client(mediator, contracts),
        HostCase.GrpcClient => GrpcClient(mediator, contracts),
        HostCase.GraphQLClient => GraphQLClient(mediator, contracts),
        _ => throw new ArgumentOutOfRangeException(nameof(host)),
    };
}

/// <summary>
///     The contracts of the diagnostic matrix in each mediator's own shapes: the rockets operations plus one request per binding problem. With
///     <c>dataContracts</c>, every carried type has <c>[DataContract]</c>/<c>[DataMember]</c>, so gRPC hosts report nothing about serialization.
/// </summary>
internal static class MatrixContracts
{
    private sealed record Shapes(string Usings, Func<string, string>? Query, Func<string, string>? Command, Func<string, string>? Stream, string? Void, bool Struct);

    private static Shapes For(Mediator mediator) => mediator switch
    {
        Mediator.MediatR => new("using MediatR;", t => $"IRequest<{t}>", t => $"IRequest<{t}>", t => $"IStreamRequest<{t}>", "IRequest", false),
        Mediator.Mediator => new("using Mediator;", t => $"IQuery<{t}>", t => $"ICommand<{t}>", t => $"IStreamQuery<{t}>", "ICommand", false),
        Mediator.ChannelMediator => new("using ChannelMediator;", t => $"IRequest<{t}>", t => $"IRequest<{t}>", t => $"IStreamRequest<{t}>", "IRequest", false),
        Mediator.ZeroAlloc => new("using ZeroAlloc.Mediator;", t => $"IRequest<{t}>", t => $"IRequest<{t}>", t => $"IStreamRequest<{t}>", "IRequest", true),
        // Foundatio's markers are optional, and it has none for streams; Immediate.Handlers has none at all: responses come from [ApiOperation].
        Mediator.Foundatio => new("using Foundatio.Mediator;", t => $"IRequest<{t}>", t => $"IRequest<{t}>", null, "IRequest<bool>", false),
        Mediator.ImmediateHandlers => new("", null, null, null, null, false),
        Mediator.Custom => new("", t => $"IMyRequest<{t}>", t => $"IMyRequest<{t}>", t => $"IMyStream<{t}>", "IMyRequest<bool>", false),
        _ => throw new ArgumentOutOfRangeException(nameof(mediator)),
    };

    private enum Shape
    {
        Query,
        Command,
        Stream,
        Void,
        None,
    }

    public static string For(Mediator mediator, bool dataContracts, string? handlerFor)
    {
        var shapes = For(mediator);
        string DataContract() => dataContracts ? "[global::System.Runtime.Serialization.DataContract] " : "";

        string Parameters(string parameters) =>
            dataContracts
                ? string.Join(", ", parameters.Split(", ").Select((p, i) => $"[property: global::System.Runtime.Serialization.DataMember(Order = {i + 1})] {p}"))
                : parameters;

        string Request(string container, string parameters, Shape shape, string response, string? extra = null)
        {
            var marker = shape switch
            {
                Shape.Query => shapes.Query?.Invoke(response),
                Shape.Command => shapes.Command?.Invoke(response),
                Shape.Stream => shapes.Stream?.Invoke(response),
                Shape.Void => shapes.Void,
                Shape.None => null,
                _ => throw new ArgumentOutOfRangeException(nameof(shape)),
            };
            var markerless = shape switch
            {
                Shape.Stream => $"System.Collections.Generic.IAsyncEnumerable<{response}>",
                Shape.Void => "bool",
                Shape.Query or Shape.Command or Shape.None => response.TrimEnd('?'),
                _ => throw new ArgumentOutOfRangeException(nameof(shape)),
            };
            var apiOperation = marker is null && shape != Shape.None ? $"[ApiOperation(Response = typeof({markerless}))] " : "";
            var kind = shapes.Struct ? "readonly record struct" : "sealed record";
            var handler = handlerFor == container ? Handler(mediator, shape, response) : "";
            return $$"""
                public static class {{container}}
                {
                    {{apiOperation}}{{DataContract()}}public {{kind}} Request({{Parameters(parameters)}}){{( marker is null ? "" : " : " + marker )}};
                    {{extra}}
                    {{handler}}
                }
                """;
        }

        var snapshot = shapes.Struct
            ? $"public readonly struct Request{Marker(shapes.Query, "RocketModel")} {{ public System.Guid Id {{ get; }} }}"
            : $"{( shapes.Query is null ? "[ApiOperation(Response = typeof(RocketModel))] " : "" )}public sealed class Request{Marker(shapes.Query, "RocketModel")} {{ public System.Guid Id {{ get; }} }}";

        return $$"""
            #nullable enable
            {{shapes.Usings}}
            using Rocket.Surgery.LaunchPad.Api;

            namespace Sample.Contracts.Rockets
            {
                {{( mediator == Mediator.Custom ? CustomInterfaces : "" )}}

                {{DataContract()}}public sealed record RocketModel({{Parameters("System.Guid Id, string Name")}});

                public readonly struct Unparsable { }

                {{Request("GetRocket", "System.Guid Id", Shape.Query, "RocketModel?")}}
                {{Request("ListRockets", "string? Name", Shape.Stream, "RocketModel")}}
                {{Request("CreateRocket", "string Name", Shape.Command, "Response", $"[Creates(typeof(GetRocket.Request))] {DataContract()}public sealed record Response({Parameters("System.Guid Id")});")}}
                {{Request("EditRocket", "System.Guid Id, string Name", Shape.Command, "RocketModel")}}
                {{Request("DeleteRocket", "System.Guid Id", Shape.Void, "")}}
                {{Request("FindRocket", "System.Guid Id", Shape.Query, "RocketModel")}}
                {{Request("LocateRocket", "[property: ApiRoute] System.Guid Id", Shape.Query, "RocketModel")}}
                {{Request("SearchRockets", "RocketModel Filter", Shape.Query, "RocketModel")}}
                {{Request("GetRocketByCode", "Unparsable Code", Shape.Query, "RocketModel")}}
                {{Request("OrphanRocket", "System.Guid Id", Shape.None, "")}}

                public static class GetRocketSnapshot
                {
                    {{snapshot}}
                }
            }
            """;
    }

    private static string Marker(Func<string, string>? shape, string response) => shape is null ? "" : " : " + shape(response);

    private const string CustomInterfaces = """
        public interface IMyRequest<T> { }
        public interface IMyStream<T> { }
        public interface IMyHandler<TRequest, TResponse> { }
        public interface IMyDispatcher
        {
            System.Threading.Tasks.Task<T> Dispatch<T>(IMyRequest<T> request, System.Threading.CancellationToken cancellationToken = default);
            System.Collections.Generic.IAsyncEnumerable<T> Stream<T>(IMyStream<T> request, System.Threading.CancellationToken cancellationToken = default);
        }
        """;

    // The server's own handler, declared next to the request the way LaunchPad's samples do it (LPAD1013). The Immediate.Handlers one has a
    // public HandleAsync: a private one in a referenced assembly isn't imported, so the check can't see it.
    private static string Handler(Mediator mediator, Shape shape, string response) => mediator switch
    {
        Mediator.MediatR =>
            $"public sealed class Handler : MediatR.IRequestHandler<Request, {response}> {{ public System.Threading.Tasks.Task<{response}> Handle(Request request, System.Threading.CancellationToken cancellationToken) => throw null!; }}",
        Mediator.Mediator =>
            $"public sealed class Handler : Mediator.{( shape == Shape.Query ? "IQueryHandler" : "ICommandHandler" )}<Request, {response}> {{ public System.Threading.Tasks.ValueTask<{response}> Handle(Request request, System.Threading.CancellationToken cancellationToken) => default; }}",
        Mediator.ChannelMediator =>
            $"public sealed class Handler : ChannelMediator.IRequestHandler<Request, {response}> {{ public System.Threading.Tasks.Task<{response}> Handle(Request request, System.Threading.CancellationToken cancellationToken) => throw null!; }}",
        Mediator.ZeroAlloc =>
            $"public sealed class Handler : ZeroAlloc.Mediator.IRequestHandler<Request, {response}> {{ public System.Threading.Tasks.ValueTask<{response}> Handle(Request request, System.Threading.CancellationToken ct) => default; }}",
        Mediator.Foundatio => $"public sealed class Handler {{ public {response} Handle(Request request) => default!; }}",
        Mediator.ImmediateHandlers =>
            $"[Immediate.Handlers.Shared.Handler] public static partial class Handler {{ public static System.Threading.Tasks.ValueTask<{response}> HandleAsync(Request request, System.Threading.CancellationToken token) => default; }}",
        Mediator.Custom => $"public sealed class Handler : IMyHandler<Request, {response}> {{ }}",
        _ => throw new ArgumentOutOfRangeException(nameof(mediator)),
    };
}
