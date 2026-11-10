// Compiled into both Rocket.Surgery.LaunchPad.Analyzers (the API generators' mediator profiles) and Rocket.Surgery.LaunchPad.Primitives.Analyzers
// (the JSON context manifest), so both recognise the same mediator requests. Plain data: no Roslyn types.
namespace Rocket.Surgery.LaunchPad.Mediators;

/// <summary>How a request interface answers: one response, none, or a stream of items.</summary>
internal enum MediatorRequestShape
{
    Unary = 1,
    Void = 2,
    Stream = 3,
}

/// <summary>
///     One kind of request a mediator library declares: the metadata name of its marker interface (null for a library whose requests carry no
///     marker), its shape, which type argument is the response, and the handler interface and method the API generators dispatch through.
/// </summary>
internal sealed record MediatorRequestInterface(
    string? MetadataName,
    MediatorRequestShape Shape,
    int ResponseTypeArgumentIndex,
    string? HandlerInterface,
    string HandleMethod
);

/// <summary>A mediator library LaunchPad supports, detected by the types it references.</summary>
internal sealed record MediatorLibrary(string Name, string[] DetectedBy, MediatorRequestInterface[] Requests);

/// <summary>The mediator libraries LaunchPad supports out of the box.</summary>
internal static class MediatorLibraries
{
    public static MediatorLibrary MediatR { get; } = new(
        "MediatR",
        ["MediatR.ISender", "MediatR.IRequest`1"],
        [
            new("MediatR.IRequest`1", MediatorRequestShape.Unary, 0, "MediatR.IRequestHandler`2", "Handle"),
            new("MediatR.IRequest", MediatorRequestShape.Void, 0, "MediatR.IRequestHandler`1", "Handle"),
            new("MediatR.IStreamRequest`1", MediatorRequestShape.Stream, 0, "MediatR.IStreamRequestHandler`2", "Handle"),
        ]
    );

    public static MediatorLibrary Mediator { get; } = new(
        "Mediator",
        ["Mediator.ISender", "Mediator.IMessage"],
        [
            new("Mediator.IRequest`1", MediatorRequestShape.Unary, 0, "Mediator.IRequestHandler`2", "Handle"),
            new("Mediator.ICommand`1", MediatorRequestShape.Unary, 0, "Mediator.ICommandHandler`2", "Handle"),
            new("Mediator.IQuery`1", MediatorRequestShape.Unary, 0, "Mediator.IQueryHandler`2", "Handle"),
            new("Mediator.IStreamRequest`1", MediatorRequestShape.Stream, 0, "Mediator.IStreamRequestHandler`2", "Handle"),
            new("Mediator.IStreamCommand`1", MediatorRequestShape.Stream, 0, "Mediator.IStreamCommandHandler`2", "Handle"),
            new("Mediator.IStreamQuery`1", MediatorRequestShape.Stream, 0, "Mediator.IStreamQueryHandler`2", "Handle"),
        ]
    );

    public static MediatorLibrary ChannelMediator { get; } = new(
        "ChannelMediator",
        ["ChannelMediator.IMediator", "ChannelMediator.IRequest`1"],
        [
            new("ChannelMediator.IRequest`1", MediatorRequestShape.Unary, 0, "ChannelMediator.IRequestHandler`2", "Handle"),
            new("ChannelMediator.IStreamRequest`1", MediatorRequestShape.Stream, 0, "ChannelMediator.IStreamRequestHandler`2", "Handle"),
        ]
    );

    // A [Handler] has no marker interface: its requests are found through its handler, not by the JSON context manifest.
    public static MediatorLibrary ImmediateHandlers { get; } = new(
        "Immediate.Handlers",
        ["Immediate.Handlers.Shared.IHandler`2"],
        [new(null, MediatorRequestShape.Unary, 0, "Immediate.Handlers.Shared.IHandler`2", "HandleAsync")]
    );

    public static MediatorLibrary Foundatio { get; } = new(
        "Foundatio.Mediator",
        ["Foundatio.Mediator.IMediator"],
        [
            new("Foundatio.Mediator.IRequest`1", MediatorRequestShape.Unary, 0, null, "HandleAsync"),
            new("Foundatio.Mediator.ICommand`1", MediatorRequestShape.Unary, 0, null, "HandleAsync"),
            new("Foundatio.Mediator.IQuery`1", MediatorRequestShape.Unary, 0, null, "HandleAsync"),
            // After the generic kinds: a void command, sent with the non-generic InvokeAsync.
            new("Foundatio.Mediator.ICommand", MediatorRequestShape.Void, 0, null, "HandleAsync"),
            new(null, MediatorRequestShape.Unary, 0, null, "HandleAsync"),
        ]
    );

    public static MediatorLibrary ZeroAlloc { get; } = new(
        "ZeroAlloc.Mediator",
        ["ZeroAlloc.Mediator.IRequest`1"],
        [
            new("ZeroAlloc.Mediator.IRequest`1", MediatorRequestShape.Unary, 0, "ZeroAlloc.Mediator.IRequestHandler`2", "Handle"),
            new("ZeroAlloc.Mediator.IStreamRequest`1", MediatorRequestShape.Stream, 0, "ZeroAlloc.Mediator.IStreamRequestHandler`2", "Handle"),
        ]
    );

    public static IReadOnlyList<MediatorLibrary> All { get; } = [MediatR, Mediator, ChannelMediator, ImmediateHandlers, Foundatio, ZeroAlloc];

    /// <summary>The metadata names of the unit types mediators answer a void request with; they carry no JSON.</summary>
    public static IReadOnlyList<string> UnitTypes { get; } = ["Mediator.Unit", "ChannelMediator.Unit", "ZeroAlloc.Mediator.Unit", "MediatR.Unit", "System.ValueTuple"];
}
