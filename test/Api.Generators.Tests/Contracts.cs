namespace Api.Generators.Tests;

/// <summary>
///     The rockets contracts in every supported mediator's own shapes, laid out the way LaunchPad's samples are: nested <c>X.Request</c> types.
/// </summary>
internal static class Contracts
{
    public static string For(Mediator mediator) => mediator switch
    {
        Mediator.MediatR => Shared("using MediatR;", "IRequest<RocketModel?>", "IStreamRequest<RocketModel>", "IRequest<Response>", "IRequest<RocketModel>", "IRequest", "record"),
        Mediator.Mediator => Shared("using Mediator;", "IQuery<RocketModel?>", "IStreamQuery<RocketModel>", "ICommand<Response>", "ICommand<RocketModel>", "ICommand", "record"),
        Mediator.ChannelMediator => Shared("using ChannelMediator;", "IRequest<RocketModel?>", "IStreamRequest<RocketModel>", "IRequest<Response>", "IRequest<RocketModel>", "IRequest", "record"),
        Mediator.ZeroAlloc => Shared("using ZeroAlloc.Mediator;", "IRequest<RocketModel?>", "IStreamRequest<RocketModel>", "IRequest<Response>", "IRequest<RocketModel>", "IRequest", "readonly record struct"),
        Mediator.Foundatio => Markerless("using Foundatio.Mediator;", "IRequest<RocketModel?>", "IRequest<Response>", "IRequest<RocketModel>", "IRequest<bool>"),
        Mediator.ImmediateHandlers => Markerless("", "", "", "", ""),
        _ => throw new ArgumentOutOfRangeException(nameof(mediator)),
    };

    private static string Shared(string usings, string get, string list, string create, string edit, string delete, string kind) => $$"""
        #nullable enable
        {{usings}}
        using Rocket.Surgery.LaunchPad.Api;

        namespace Sample.Contracts.Rockets
        {
            public sealed record RocketModel(System.Guid Id, string Name);

            public static class GetRocket
            {
                [ApiOperation(Summary = "Load a rocket")]
                public {{kind}} Request(System.Guid Id) : {{get}};
            }

            public static class ListRockets { public {{kind}} Request(string? Name) : {{list}}; }

            public static class CreateRocket
            {
                public {{kind}} Request(string Name) : {{create}};
                public sealed record Response(System.Guid Id);
            }

            public static class EditRocket { public {{kind}} Request(System.Guid Id, string Name) : {{edit}}; }

            public static class DeleteRocket { public {{kind}} Request(System.Guid Id) : {{delete}}; }
        }
        """;

    // Foundatio's markers are optional and Immediate.Handlers has none: responses come from [ApiOperation(Response = ...)].
    private static string Markerless(string usings, string get, string create, string edit, string delete)
    {
        static string Marker(string marker, string response) => marker.Length > 0 ? " : " + marker : "";
        static string Response(string marker, string response) => marker.Length > 0 ? "" : $", Response = typeof({response})";
        return $$"""
            #nullable enable
            {{usings}}
            using Rocket.Surgery.LaunchPad.Api;

            namespace Sample.Contracts.Rockets
            {
                public sealed record RocketModel(System.Guid Id, string Name);

                public static class GetRocket
                {
                    [ApiOperation(Summary = "Load a rocket"{{Response(get, "RocketModel")}})]
                    public sealed record Request(System.Guid Id){{Marker(get, "")}};
                }

                public static class ListRockets
                {
                    [ApiOperation(Response = typeof(System.Collections.Generic.IAsyncEnumerable<RocketModel>))]
                    public sealed record Request(string? Name);
                }

                public static class CreateRocket
                {
                    [ApiOperation(Name = "CreateRocket"{{Response(create, "Response")}})]
                    public sealed record Request(string Name){{Marker(create, "")}};
                    public sealed record Response(System.Guid Id);
                }

                public static class EditRocket
                {
                    [ApiOperation(Name = "EditRocket"{{Response(edit, "RocketModel")}})]
                    public sealed record Request(System.Guid Id, string Name){{Marker(edit, "")}};
                }

                public static class DeleteRocket
                {
                    [ApiOperation(Name = "DeleteRocket"{{Response(delete, "bool")}})]
                    public sealed record Request(System.Guid Id){{Marker(delete, "")}};
                }
            }
            """;
    }

    public const string ServerHost = """
        #nullable enable
        using Rocket.Surgery.LaunchPad.Api;
        using Sample.Contracts.Rockets;

        namespace Sample.Server
        {
            [Operation<GetRocket.Request>]
            [Operation<ListRockets.Request>]
            [CreateOperation<CreateRocket.Request, GetRocket.Request>]
            [Operation<EditRocket.Request>]
            [Operation<DeleteRocket.Request>]
            public static partial class RocketsEndpoints;
        }
        """;

    public const string ClientHost = """
        #nullable enable
        using Rocket.Surgery.LaunchPad.Api;
        using Sample.Contracts.Rockets;

        namespace Sample.Client
        {
            [Operation<GetRocket.Request>]
            [Operation<ListRockets.Request>]
            [Operation<CreateRocket.Request>]
            [Operation<EditRocket.Request>]
            [Operation<DeleteRocket.Request>]
            public partial class RocketsClient;
        }
        """;

    public const string ControllerHost = """
        #nullable enable
        using Rocket.Surgery.LaunchPad.Api;
        using Sample.Contracts.Rockets;

        namespace Sample.Server
        {
            [Operation<GetRocket.Request>]
            [Operation<ListRockets.Request>]
            [CreateOperation<CreateRocket.Request, GetRocket.Request>]
            [Operation<EditRocket.Request>]
            [Operation<DeleteRocket.Request>]
            public partial class RocketsController;
        }
        """;
}
