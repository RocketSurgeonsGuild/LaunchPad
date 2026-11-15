using Microsoft.CodeAnalysis.CSharp;

namespace Api.Generators.Tests;

/// <summary>
///     Union responses on every transport and with every built-in profile: the value case answers as a plain response would, each error case
///     in the transport's error shape with the status its name or <c>[ApiError]</c> gives it.
/// </summary>
public class UnionGeneratorTests() : GeneratorTest()
{
    public static IEnumerable<(HostCase Host, Mediator Mediator)> Cases() =>
        from host in Enum.GetValues<HostCase>()
        from mediator in new[] { Mediator.Mediator, Mediator.ChannelMediator, Mediator.ImmediateHandlers, Mediator.Foundatio, Mediator.ZeroAlloc }
        select (host, mediator);

    [Test]
    [MethodDataSource(nameof(Cases))]
    public async Task Should_Answer_Union_Cases(HostCase host, Mediator mediator)
    {
        var contracts = await CompileContracts(mediator, UnionContracts.For(mediator), typeof(System.Runtime.Serialization.DataContractAttribute).Assembly);
        var builder = host switch
        {
            HostCase.MinimalApi => Server(mediator, contracts),
            HostCase.Controller => Controllers(mediator, contracts),
            HostCase.GrpcService => GrpcServer(mediator, contracts),
            HostCase.GraphQLMutation => GraphQLServer(mediator, contracts),
            _ => throw new ArgumentOutOfRangeException(nameof(host)),
        };
        var result = await builder.AddSources(UnionContracts.Host(host)).Build().GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseParameters(host, mediator);
    }

    // Every name rule and the endings a name may carry: the generated Minimal API endpoint shows the status each case answers.
    [Test]
    public async Task Should_Map_Case_Names()
    {
        var contracts = await CompileContracts(
            Mediator.Mediator,
            $$"""
            #nullable enable
            using Mediator;

            namespace Sample.Contracts
            {
                public sealed record RocketModel(System.Guid Id);
                public sealed record NotFound;
                public sealed record RocketNotFoundError(System.Guid Id);
                public sealed record InvalidName(string Name);
                public sealed record ValidationProblem(string[] Errors);
                public sealed record Unauthorized;
                public sealed record Unauthenticated;
                public sealed record ForbiddenException;
                public sealed record NameConflictFailure(string Name);

                [System.Runtime.CompilerServices.Union]
                public readonly struct Result : System.Runtime.CompilerServices.IUnion
                {
                    public Result(RocketModel value) => Value = value;
                    public Result(NotFound value) => Value = value;
                    public Result(RocketNotFoundError value) => Value = value;
                    public Result(InvalidName value) => Value = value;
                    public Result(ValidationProblem value) => Value = value;
                    public Result(Unauthorized value) => Value = value;
                    public Result(Unauthenticated value) => Value = value;
                    public Result(ForbiddenException value) => Value = value;
                    public Result(NameConflictFailure value) => Value = value;
                    public object? Value { get; }
                }

                public static class GetRocket { public sealed record Request(System.Guid Id) : IRequest<Result>; }
            }
            """
        );
        var result = await Server(Mediator.Mediator, contracts)
                          .AddSources("using Rocket.Surgery.LaunchPad.Api;\nusing Sample.Contracts;\n[Operation<GetRocket.Request>]\npublic static partial class RocketsEndpoints;")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    // A native union declared in the server's own project. In source, the compiler declares its case constructors implicitly, unlike those of
    // a union read from a contracts assembly or a hand-written one.
    [Test]
    public async Task Should_Answer_A_Native_Union_Declared_In_The_Server()
    {
        var contracts = await CompileContracts(Mediator.Mediator, "namespace Sample.Contracts { public sealed record Placeholder; }");
        var result = await Server(Mediator.Mediator, contracts)
                          .WithLanguageVersion(LanguageVersion.Preview)
                          .AddSources(
                               """
                               #nullable enable
                               using Mediator;
                               using Rocket.Surgery.LaunchPad.Api;

                               namespace Sample.Server
                               {
                                   public sealed record RocketModel(System.Guid Id);
                                   public sealed record RocketNotFound(System.Guid Id);
                                   public union GetRocketResult(RocketModel, RocketNotFound);

                                   public static class GetRocket { public sealed record Request(System.Guid Id) : IRequest<GetRocketResult>; }

                                   [Operation<GetRocket.Request>]
                                   public static partial class RocketsEndpoints;
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    // A union whose value case is nullable answers 404 for null, as a nullable response does.
    [Test]
    public async Task Should_Answer_Not_Found_For_A_Null_Value_Case()
    {
        var contracts = await CompileContracts(
            Mediator.Mediator,
            $$"""
            #nullable enable
            using Mediator;

            namespace Sample.Contracts
            {
                public sealed record RocketModel(System.Guid Id);
                public sealed record Forbidden;

                [System.Runtime.CompilerServices.Union]
                public readonly struct Result : System.Runtime.CompilerServices.IUnion
                {
                    public Result(RocketModel? value) => Value = value;
                    public Result(Forbidden value) => Value = value;
                    public object? Value { get; }
                }

                public static class GetRocket { public sealed record Request(System.Guid Id) : IRequest<Result>; }

                // The union itself is nullable: null answers 404 too.
                public static class FindRocket { public sealed record Request(System.Guid Id) : IRequest<Result?>; }
            }
            """
        );
        var result = await Server(Mediator.Mediator, contracts)
                          .AddSources("using Rocket.Surgery.LaunchPad.Api;\nusing Sample.Contracts;\n[Operation<GetRocket.Request>]\n[Operation<FindRocket.Request>(Route = \"find/{id}\")]\npublic static partial class RocketsEndpoints;")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    // A native `union` declaration in another assembly: Roslyn doesn't surface its [Union], so the generators find it through IUnion.
    [Test]
    public async Task Should_Answer_A_Native_Union_From_Another_Assembly()
    {
        var contracts = await CompileContracts(
            Mediator.Mediator,
            $$"""
            #nullable enable
            using Mediator;

            namespace Sample.Contracts
            {
                public sealed record RocketModel(System.Guid Id);
                public sealed record NotFound(System.Guid Id);
                public union GetRocketResult(RocketModel, NotFound);
                public static class GetRocket { public sealed record Request(System.Guid Id) : IRequest<GetRocketResult>; }
            }
            """
        );
        var result = await Server(Mediator.Mediator, contracts)
                          .AddSources("using Rocket.Surgery.LaunchPad.Api;\nusing Sample.Contracts;\n[Operation<GetRocket.Request>]\npublic static partial class RocketsEndpoints;")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result);
    }

    // LaunchPad's standard problem cases, stood in for by name: a Problem case answers each standard case by its runtime type, then any other
    // problem as 500, and every case carries its problem data (detail, title, type, instance, properties) in the transport's error shape.
    public const string Problems = """
        namespace Rocket.Surgery.LaunchPad.Foundation
        {
            public interface IProblemDetailsData
            {
                System.Collections.Generic.IDictionary<string, object?> Properties { get; }
                string? Title { get; }
                string? Link { get; }
                string? Instance { get; }
            }

            public abstract record Problem(string? Detail = null) : IProblemDetailsData
            {
                public string? Title { get; init; }
                public string? Link { get; init; }
                public string? Instance { get; init; }
                public System.Collections.Generic.IDictionary<string, object?> Properties { get; init; } = new System.Collections.Generic.Dictionary<string, object?>();
            }

            public sealed record NotFound(string? Detail = null) : Problem(Detail);
            public sealed record Invalid(string? Detail = null) : Problem(Detail)
            {
                public System.Collections.Generic.IReadOnlyDictionary<string, string[]> Errors { get; init; } = new System.Collections.Generic.Dictionary<string, string[]>();
            }
            public sealed record UnprocessableEntity(string? Detail = null) : Problem(Detail)
            {
                public System.Collections.Generic.IReadOnlyDictionary<string, string[]> Errors { get; init; } = new System.Collections.Generic.Dictionary<string, string[]>();
            }
            public sealed record Unauthorized(string? Detail = null) : Problem(Detail);
            public sealed record Forbidden(string? Detail = null) : Problem(Detail);
            public sealed record Conflict(string? Detail = null) : Problem(Detail);
        }
        """;

    public static IEnumerable<HostCase> Hosts() => Enum.GetValues<HostCase>();

    [Test]
    [MethodDataSource(nameof(Hosts))]
    public async Task Should_Answer_Problem_Cases(HostCase host)
    {
        var contracts = await CompileContracts(
            Mediator.Mediator,
            $$"""
            #nullable enable
            using Mediator;
            using Rocket.Surgery.LaunchPad.Foundation;


            {{Problems}}

            namespace Sample.Contracts
            {
                [System.Runtime.Serialization.DataContract]
                public sealed record RocketModel([property: System.Runtime.Serialization.DataMember(Order = 1)] string Id);

                [System.Runtime.CompilerServices.Union]
                public readonly struct GetRocketResult : System.Runtime.CompilerServices.IUnion
                {
                    public GetRocketResult(RocketModel value) => Value = value;
                    public GetRocketResult(Invalid value) => Value = value;
                    public GetRocketResult(Problem value) => Value = value;
                    public object? Value { get; }
                }

                public static class UpdateRocket
                {
                    [System.Runtime.Serialization.DataContract]
                    public sealed record Request([property: System.Runtime.Serialization.DataMember(Order = 1)] string Id) : IRequest<GetRocketResult>;
                }
            }
            """,
            typeof(System.Runtime.Serialization.DataContractAttribute).Assembly
        );
        var builder = host switch
        {
            HostCase.MinimalApi => Server(Mediator.Mediator, contracts),
            HostCase.Controller => Controllers(Mediator.Mediator, contracts),
            HostCase.GrpcService => GrpcServer(Mediator.Mediator, contracts),
            HostCase.GraphQLMutation => GraphQLServer(Mediator.Mediator, contracts),
            _ => throw new ArgumentOutOfRangeException(nameof(host)),
        };
        var declaration = host switch
        {
            HostCase.MinimalApi => "public static partial class RocketsEndpoints;",
            HostCase.Controller => "public partial class RocketsController;",
            HostCase.GrpcService => "public partial class RocketsService;",
            HostCase.GraphQLMutation => "public partial class RocketsMutations;",
            _ => throw new ArgumentOutOfRangeException(nameof(host)),
        };
        var result = await builder
                          .AddSources($"using Rocket.Surgery.LaunchPad.Api;\nusing Sample.Contracts;\nnamespace Sample.Server\n{{\n    [Operation<UpdateRocket.Request>]\n    {declaration}\n}}")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseParameters(host);
    }
}

/// <summary>
///     Union contracts in each mediator's own shapes, as native C# 15 <c>union</c> declarations. The other tests here declare hand-written
///     custom unions (a <c>[Union]</c> struct implementing <c>IUnion</c>), which the generators detect the same way.
/// </summary>
internal static class UnionContracts
{
    internal sealed record Shape(string Usings, Func<string, string>? Marker, Func<string, string>? Stream, string Kind, string Unit);

    internal static Shape ShapeOf(Mediator mediator) => mediator switch
    {
        Mediator.Mediator => new("using Mediator;", t => $"ICommand<{t}>", t => $"IStreamQuery<{t}>", "sealed record", "Mediator.Unit"),
        Mediator.ChannelMediator => new("using ChannelMediator;", t => $"IRequest<{t}>", t => $"IStreamRequest<{t}>", "sealed record", "ChannelMediator.Unit"),
        Mediator.ZeroAlloc => new("using ZeroAlloc.Mediator;", t => $"IRequest<{t}>", t => $"IStreamRequest<{t}>", "readonly record struct", "ZeroAlloc.Mediator.Unit"),
        // Foundatio has no stream marker: the stream's response comes from [ApiOperation(Response)].
        Mediator.Foundatio => new("using Foundatio.Mediator;", t => $"IRequest<{t}>", null, "sealed record", "System.ValueTuple"),
        // No marker: the response comes from [ApiOperation(Response)].
        Mediator.ImmediateHandlers => new("", null, null, "sealed record", "System.ValueTuple"),
        Mediator.Custom => throw new ArgumentOutOfRangeException(nameof(mediator)),
        _ => throw new ArgumentOutOfRangeException(nameof(mediator)),
    };

    public static string For(Mediator mediator)
    {
        var shape = ShapeOf(mediator);

        string Request(string container, string parameters, string response, string? extra = null, bool stream = false)
        {
            var marker = stream ? shape.Stream : shape.Marker;
            var markerless = stream ? $"System.Collections.Generic.IAsyncEnumerable<{response}>" : response;
            return $$"""
                public static class {{container}}
                {
                    {{( marker is null ? $"[ApiOperation(Response = typeof({markerless}))] " : "" )}}[DataContract] public {{shape.Kind}} Request({{parameters}}){{( marker is { } m ? " : " + m(response) : "" )}};
                    {{extra}}
                }
                """;
        }

        static string Union(string name, params string[] cases) => $"public union {name}({string.Join(", ", cases)});";

        return $$"""
            #nullable enable
            {{shape.Usings}}
            using System.Runtime.Serialization;
            using Rocket.Surgery.LaunchPad.Api;

            namespace Sample.Contracts.Rockets
            {
                [DataContract] public sealed record RocketModel([property: DataMember(Order = 1)] System.Guid Id, [property: DataMember(Order = 2)] string Name);

                [DataContract] public sealed record RocketNotFound([property: DataMember(Order = 1)] System.Guid Id);
                public sealed record Forbidden;
                [DataContract] public sealed record ValidationFailed([property: DataMember(Order = 1)] string[] Errors);
                [DataContract] public sealed record NameConflict([property: DataMember(Order = 1)] string Name);
                [ApiError(422, GrpcStatus = 9, GraphQLCode = "UNPROCESSABLE")]
                [DataContract] public sealed record NotReady([property: DataMember(Order = 1)] string Reason);

                {{Union("GetRocketResult", "RocketModel", "RocketNotFound", "Forbidden")}}
                {{Union("CreateRocketResult", "CreateRocket.Response", "ValidationFailed", "NameConflict")}}
                {{Union("DeleteRocketResult", shape.Unit, "RocketNotFound")}}
                {{Union("LaunchRocketResult", "RocketModel", "NotReady", "RocketNotFound")}}

                {{Request("GetRocket", "[property: DataMember(Order = 1)] System.Guid Id", "GetRocketResult")}}
                {{Request("CreateRocket", "[property: DataMember(Order = 1)] string Name", "CreateRocketResult", "[Creates(typeof(GetRocket.Request))] [DataContract] public sealed record Response([property: DataMember(Order = 1)] System.Guid Id);")}}
                {{Request("DeleteRocket", "[property: DataMember(Order = 1)] System.Guid Id", "DeleteRocketResult")}}
                {{Request("LaunchRocket", "[property: DataMember(Order = 1)] System.Guid Id", "LaunchRocketResult")}}
                {{Request("ListLaunches", "[property: DataMember(Order = 1)] string? Name", "LaunchRocketResult", stream: true)}}
            }
            """;
    }

    /// <summary>A host listing the union operations; GraphQL mutation hosts leave out the query.</summary>
    public static string Host(HostCase host)
    {
        var declaration = host switch
        {
            HostCase.MinimalApi => "public static partial class RocketsEndpoints;",
            HostCase.Controller => "public partial class RocketsController;",
            HostCase.GrpcService => "public partial class RocketsService;",
            HostCase.GraphQLMutation => "public partial class RocketsMutations;",
            _ => throw new ArgumentOutOfRangeException(nameof(host)),
        };
        return $$"""
            #nullable enable
            using Rocket.Surgery.LaunchPad.Api;
            using Sample.Contracts.Rockets;

            namespace Sample.Server
            {
                {{( host == HostCase.GraphQLMutation ? "" : "[Operation<GetRocket.Request>]\n    [Operation<ListLaunches.Request>]" )}}
                [Operation<CreateRocket.Request>]
                [Operation<DeleteRocket.Request>]
                [Operation<LaunchRocket.Request>]
                {{declaration}}
            }
            """;
    }
}
