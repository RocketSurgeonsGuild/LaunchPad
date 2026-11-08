using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Rocket.Surgery.LaunchPad.Api.Generators.ApiSyntax;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>How a server reaches the mediator for one operation: the injected service and the call, written to match the profile's shape.</summary>
internal static class ServerDispatch
{
    public const string SenderParameter = "__sender";

    /// <summary>The injected service's type, or null when the profile can't dispatch this operation.</summary>
    public static string? ServiceType(ProfileModel profile, OperationModel operation) => profile.Dispatch switch
    {
        DispatchKind.Sender => profile.SenderType,
        DispatchKind.HandlerInterface => operation.HandlerInterface,
        _ => throw new ArgumentOutOfRangeException(nameof(profile)),
    };

    /// <summary>
    ///     The un-awaited call that sends <paramref name="request" />: a Task/ValueTask for unary and void operations, and for a stream either
    ///     the stream itself or (<c>Awaited</c>) an awaitable that produces it (Foundatio's InvokeAsync&lt;IAsyncEnumerable&lt;T&gt;&gt;,
    ///     Immediate.Handlers' HandleAsync).
    /// </summary>
    public static (ExpressionSyntax Call, bool Awaited) Send(ProfileModel profile, OperationModel operation, ExpressionSyntax request, ExpressionSyntax cancellationToken)
    {
        var sender = Id(SenderParameter);
        if (profile.Dispatch == DispatchKind.HandlerInterface)
        {
            return (Invoke(Member(sender, profile.SendMethod), request, cancellationToken), operation.Kind == RequestKind.Stream);
        }

        if (operation.Kind == RequestKind.Stream)
        {
            return profile.StreamMethod is { } stream
                ? (Invoke(Member(sender, stream), request, cancellationToken), false)
                : (Invoke(GenericMember(sender, profile.SendMethod, AsyncEnumerable(Type(operation.ResponseTypeNotNull!))), request, cancellationToken), true);
        }

        // A request with no marker interface can't let the sender infer its response, so the call names it (Foundatio's InvokeAsync<T>).
        var method = ( profile.SendNeedsResponseTypeArgument || operation.Markerless ) && operation.Kind == RequestKind.Unary
            ? GenericMember(sender, profile.SendMethod, Type(operation.ResponseType!))
            : Member(sender, profile.SendMethod);
        return (Invoke(method, request, cancellationToken), false);
    }

    /// <summary>The stream a stream operation produces: the call itself, or the awaited call.</summary>
    public static ExpressionSyntax Stream((ExpressionSyntax Call, bool Awaited) send) => send.Awaited ? Await(send.Call) : send.Call;
}
