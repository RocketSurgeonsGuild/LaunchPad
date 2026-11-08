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
    ///     The expression that sends <paramref name="request" />: awaitable for unary and void operations, an IAsyncEnumerable for streams.
    /// </summary>
    public static string Call(ProfileModel profile, OperationModel operation, string request, string cancellationToken)
    {
        if (profile.Dispatch == DispatchKind.HandlerInterface)
        {
            // A handler-interface stream (Immediate.Handlers) returns ValueTask<IAsyncEnumerable<T>>.
            var call = $"{SenderParameter}.{profile.SendMethod}({request}, {cancellationToken})";
            return operation.Kind == RequestKind.Stream ? $"await {call}.ConfigureAwait(false)" : call;
        }

        if (operation.Kind == RequestKind.Stream)
        {
            return profile.StreamMethod is { } stream
                ? $"{SenderParameter}.{stream}({request}, {cancellationToken})"
                // Foundatio streams through InvokeAsync<IAsyncEnumerable<T>>.
                : $"await {SenderParameter}.{profile.SendMethod}<global::System.Collections.Generic.IAsyncEnumerable<{operation.ResponseTypeNotNull}>>({request}, {cancellationToken}).ConfigureAwait(false)";
        }

        // A request with no marker interface can't let the sender infer its response, so the call names it (Foundatio's InvokeAsync<T>).
        var typeArgument = ( profile.SendNeedsResponseTypeArgument || operation.Markerless ) && operation.Kind == RequestKind.Unary ? $"<{operation.ResponseType}>" : "";
        return $"{SenderParameter}.{profile.SendMethod}{typeArgument}({request}, {cancellationToken})";
    }
}
