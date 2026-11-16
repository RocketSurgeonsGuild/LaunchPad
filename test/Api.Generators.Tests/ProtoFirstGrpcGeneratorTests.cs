namespace Api.Generators.Tests;

/// <summary>
///     Proto-first gRPC hosts: a host deriving the service base Grpc.Tools generated from a <c>.proto</c> gets an override per listed operation,
///     mapping the proto messages through the ZeroAlloc.Mapping mappings the user declares.
/// </summary>
/// <remarks>
///     Grpc.Tools and ZeroAlloc.Mapping's generator don't run in these tests: <see cref="ProtoFirst" /> writes out an abridged version of what
///     they would generate (the messages, the service base, the <c>Map</c> methods).
/// </remarks>
public class ProtoFirstGrpcGeneratorTests() : GeneratorTest()
{
    [Test]
    [Arguments(Mediator.Mediator)]
    [Arguments(Mediator.ChannelMediator)]
    [Arguments(Mediator.ImmediateHandlers)]
    [Arguments(Mediator.Foundatio)]
    [Arguments(Mediator.ZeroAlloc)]
    public async Task Should_Generate_A_Service(Mediator mediator)
    {
        var contracts = await CompileContracts(mediator, Contracts.For(mediator));
        var result = await ProtoGrpcServer(mediator, contracts)
                          .AddSources(ProtoFirst.RocketsProto, ProtoFirst.RocketsMappings, ProtoFirst.Host(ProtoFirst.RocketsOperations))
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseParameters(mediator);
    }

    // The union operations: a unary union, a stream of unions and a union whose value case is void. NotReady is mapped to a proto message, so it
    // travels in the case-bin trailer; RocketNotFound isn't, so only its name and status do.
    [Test]
    public async Task Should_Answer_Union_Cases()
    {
        var contracts = await CompileContracts(Mediator.Mediator, UnionContracts.For(Mediator.Mediator), typeof(System.Runtime.Serialization.DataContractAttribute).Assembly);
        var result = await ProtoGrpcServer(Mediator.Mediator, contracts)
                          .AddSources(ProtoFirst.LaunchesProto, ProtoFirst.LaunchesMappings(), ProtoFirst.Host("[Operation<LaunchRocket.Request>]\n[Operation<ListLaunches.Request>]\n[Operation<DeleteRocket.Request>]", "Launches"))
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    [Test]
    [Arguments("LPAD1040")]
    [Arguments("LPAD1041")]
    [Arguments("LPAD1042")]
    [Arguments("LPAD1043")]
    [Arguments("LPAD1044")]
    [Arguments("LPAD1021")]
    public async Task Should_Report(string id)
    {
        if (id == "LPAD1044")
        {
            await ShouldReportInPlace();
            return;
        }

        var contracts = await CompileContracts(Mediator.Mediator, Contracts.For(Mediator.Mediator));
        string[] sources = id switch
        {
            // The operation's name matches no rpc of the service base.
            "LPAD1040" => [ProtoFirst.RocketsProto, ProtoFirst.RocketsMappings, ProtoFirst.Host("[Operation<GetRocket.Request>(Name = \"FindRocket\")]")],
            // The project doesn't reference ZeroAlloc.Mapping (and so declares no mappings).
            "LPAD1041" => [ProtoFirst.RocketsProto, ProtoFirst.Host("[Operation<GetRocket.Request>]")],
            // A stream operation on a unary rpc.
            "LPAD1042" => [ProtoFirst.RocketsProto, ProtoFirst.RocketsMappings, ProtoFirst.Host("[Operation<ListRockets.Request>(Name = \"GetRocket\")]")],
            // No mappings declared: the request's and the response's are both reported.
            "LPAD1043" => [ProtoFirst.RocketsProto, ProtoFirst.Host("[Operation<GetRocket.Request>]")],
            // The service's name comes from the .proto, so Group doesn't apply.
            "LPAD1021" => [ProtoFirst.RocketsProto, ProtoFirst.RocketsMappings, ProtoFirst.Host("[OperationHost(Group = \"fleet\")]\n[Operation<GetRocket.Request>]")],
            _ => throw new ArgumentOutOfRangeException(nameof(id), id, null),
        };
        var result = await ProtoGrpcServer(Mediator.Mediator, contracts, mapping: id != "LPAD1041").AddSources(sources).Build().GenerateAsync(TestContext.CancellationToken);

        await VerifyDiagnostics(result, settings => settings.UseParameters(id));
    }

    // An error case mapped to a proto message without the update-in-place form, which ZeroAlloc.Mapping's constructor form would leave empty.
    private async Task ShouldReportInPlace()
    {
        var contracts = await CompileContracts(Mediator.Mediator, UnionContracts.For(Mediator.Mediator), typeof(System.Runtime.Serialization.DataContractAttribute).Assembly);
        var result = await ProtoGrpcServer(Mediator.Mediator, contracts)
                          .AddSources(ProtoFirst.LaunchesProto, ProtoFirst.LaunchesMappings(inPlace: false), ProtoFirst.Host("[Operation<LaunchRocket.Request>]", "Launches"))
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await VerifyDiagnostics(result, settings => settings.UseParameters("LPAD1044"));
    }
}

/// <summary>The proto side of the proto-first tests: Grpc.Tools' output for the rockets and launches services, abridged, and the mappings.</summary>
internal static class ProtoFirst
{
    public const string RocketsOperations = """
        [Operation<GetRocket.Request>]
        [Operation<ListRockets.Request>]
        [Operation<CreateRocket.Request>]
        [Operation<EditRocket.Request>]
        [Operation<DeleteRocket.Request>]
        """;

    public static string Host(string operations, string noun = "Rockets") => $$"""
        #nullable enable
        using Rocket.Surgery.LaunchPad.Api;
        using Sample.Contracts.Rockets;

        namespace Sample.Server
        {
            {{operations}}
            public partial class {{noun}}Service : Sample.Protos.{{noun}}.{{noun}}Base;
        }
        """;

    private const string Unimplemented = "throw new global::Grpc.Core.RpcException(new global::Grpc.Core.Status(global::Grpc.Core.StatusCode.Unimplemented, \"\"))";

    private static string Unary(string name, string request, string response) =>
        $"public virtual global::System.Threading.Tasks.Task<{response}> {name}({request} request, global::Grpc.Core.ServerCallContext context) => {Unimplemented};";

    private static string ServerStreaming(string name, string request, string response) =>
        $"public virtual global::System.Threading.Tasks.Task {name}({request} request, global::Grpc.Core.IServerStreamWriter<{response}> responseStream, global::Grpc.Core.ServerCallContext context) => {Unimplemented};";

    private static string Service(string name, params string[] rpcs) => $$"""
        public static partial class {{name}}
        {
            [global::Grpc.Core.BindServiceMethod(typeof({{name}}), "BindService")]
            public abstract partial class {{name}}Base
            {
                {{string.Join("\n        ", rpcs)}}
            }

            public static global::Grpc.Core.ServerServiceDefinition BindService({{name}}Base serviceImpl) => global::Grpc.Core.ServerServiceDefinition.CreateBuilder().Build();
        }
        """;

    public static readonly string RocketsProto = $$"""
        #nullable enable
        namespace Sample.Protos
        {
            public sealed partial class GetRocketRequest { public string Id { get; set; } = ""; }
            public sealed partial class ListRocketsRequest { public string Name { get; set; } = ""; }
            public sealed partial class CreateRocketRequest { public string Name { get; set; } = ""; }
            public sealed partial class CreateRocketReply { public string Id { get; set; } = ""; }
            public sealed partial class EditRocketRequest { public string Id { get; set; } = ""; public string Name { get; set; } = ""; }
            public sealed partial class DeleteRocketRequest { public string Id { get; set; } = ""; }
            public sealed partial class RocketReply { public string Id { get; set; } = ""; public string Name { get; set; } = ""; }
            public sealed partial class Empty;

            {{Service(
                "Rockets",
                Unary("GetRocket", "GetRocketRequest", "RocketReply"),
                ServerStreaming("ListRockets", "ListRocketsRequest", "RocketReply"),
                Unary("CreateRocket", "CreateRocketRequest", "CreateRocketReply"),
                Unary("EditRocket", "EditRocketRequest", "RocketReply"),
                Unary("DeleteRocket", "DeleteRocketRequest", "Empty")
            )}}
        }
        """;

    // The mappings the user declares, then what ZeroAlloc.Mapping would generate for them: RocketReply is filled in place (the partial void Map),
    // CreateRocketReply through the constructor form. The markerless profiles' DeleteRocket answers a bool.
    public const string RocketsMappings = """
        #nullable enable
        using Sample.Contracts.Rockets;
        using Sample.Protos;
        using ZeroAlloc.Mapping;

        namespace Sample.Server
        {
            [Map<GetRocketRequest, GetRocket.Request>]
            [Map<ListRocketsRequest, ListRockets.Request>]
            [Map<CreateRocketRequest, CreateRocket.Request>]
            [Map<EditRocketRequest, EditRocket.Request>]
            [Map<DeleteRocketRequest, DeleteRocket.Request>]
            [Map<RocketModel, RocketReply>]
            [Map<CreateRocket.Response, CreateRocketReply>]
            [Map<bool, Empty>]
            public static partial class RocketMappings;

            public static partial class RocketMappings
            {
                public static GetRocket.Request Map(GetRocketRequest src) => new(System.Guid.Parse(src.Id));
                public static ListRockets.Request Map(ListRocketsRequest src) => new(src.Name);
                public static CreateRocket.Request Map(CreateRocketRequest src) => new(src.Name);
                public static EditRocket.Request Map(EditRocketRequest src) => new(System.Guid.Parse(src.Id), src.Name);
                public static DeleteRocket.Request Map(DeleteRocketRequest src) => new(System.Guid.Parse(src.Id));
                public static partial void Map(RocketModel src, RocketReply existingDst);
                public static partial void Map(RocketModel src, RocketReply existingDst) { existingDst.Id = src.Id.ToString(); existingDst.Name = src.Name; }
                public static CreateRocketReply Map(CreateRocket.Response src) => new() { Id = src.Id.ToString() };
                public static Empty Map(bool src) => new();
            }
        }
        """;

    public static readonly string LaunchesProto = $$"""
        #nullable enable
        namespace Sample.Protos
        {
            public sealed partial class LaunchRocketRequest { public string Id { get; set; } = ""; }
            public sealed partial class ListLaunchesRequest { public string Name { get; set; } = ""; }
            public sealed partial class DeleteRocketRequest { public string Id { get; set; } = ""; }
            public sealed partial class RocketReply { public string Id { get; set; } = ""; public string Name { get; set; } = ""; }
            public sealed partial class Empty;

            // A proto message, so an error case mapped to it travels in the case-bin trailer.
            public sealed partial class NotReadyError : global::Google.Protobuf.IMessage
            {
                public string Reason { get; set; } = "";
                global::Google.Protobuf.Reflection.MessageDescriptor global::Google.Protobuf.IMessage.Descriptor => throw new global::System.NotSupportedException();
                public void MergeFrom(global::Google.Protobuf.CodedInputStream input) => throw new global::System.NotSupportedException();
                public void WriteTo(global::Google.Protobuf.CodedOutputStream output) => throw new global::System.NotSupportedException();
                public int CalculateSize() => throw new global::System.NotSupportedException();
            }

            {{Service(
                "Launches",
                Unary("LaunchRocket", "LaunchRocketRequest", "RocketReply"),
                ServerStreaming("ListLaunches", "ListLaunchesRequest", "RocketReply"),
                Unary("DeleteRocket", "DeleteRocketRequest", "Empty")
            )}}
        }
        """;

    // NotReadyError is a proto message: without its update-in-place declaration, LPAD1044.
    public static string LaunchesMappings(bool inPlace = true) => $$"""
        #nullable enable
        using Sample.Contracts.Rockets;
        using Sample.Protos;
        using ZeroAlloc.Mapping;

        namespace Sample.Server
        {
            [Map<LaunchRocketRequest, LaunchRocket.Request>]
            [Map<ListLaunchesRequest, ListLaunches.Request>]
            [Map<DeleteRocketRequest, DeleteRocket.Request>]
            [Map<RocketModel, RocketReply>]
            [Map<NotReady, NotReadyError>]
            public static partial class LaunchMappings;

            public static partial class LaunchMappings
            {
                public static LaunchRocket.Request Map(LaunchRocketRequest src) => new(System.Guid.Parse(src.Id));
                public static ListLaunches.Request Map(ListLaunchesRequest src) => new(src.Name);
                public static DeleteRocket.Request Map(DeleteRocketRequest src) => new(System.Guid.Parse(src.Id));
                public static RocketReply Map(RocketModel src) => new() { Id = src.Id.ToString(), Name = src.Name };
                public static NotReadyError Map(NotReady src) => new() { Reason = src.Reason };
                {{( inPlace ? "public static partial void Map(NotReady src, NotReadyError existingDst);\n        public static partial void Map(NotReady src, NotReadyError existingDst) => existingDst.Reason = src.Reason;" : "" )}}
            }
        }
        """;
}
