using Grpc.Core;

using Rocket.Surgery.LaunchPad.Foundation;

using Sample.Core.Operations;

namespace Sample.Grpc.Services;

/// <summary>
///     Answers a union response in a proto-first gRPC service: its value case is returned, and its error case is answered with the gRPC status of
///     the case, the same status the generated gRPC servers answer it with.
/// </summary>
internal static class GrpcUnionExtensions
{
    /// <summary>The value case of a union response's <c>Value</c>; an error case ends the call with its gRPC status.</summary>
    /// <typeparam name="T">The value case.</typeparam>
    /// <param name="value">The union's <c>Value</c>.</param>
    public static T ValueOrStatus<T>(this object? value) =>
        value.TryGetValue<T>(out var result, out var problem) ? result : throw new RpcException(new(StatusCodeOf(problem), problem.Message()));

    private static StatusCode StatusCodeOf(Problem problem) => problem switch
    {
        NotFound => StatusCode.NotFound,
        Invalid or UnprocessableEntity => StatusCode.InvalidArgument,
        Unauthorized => StatusCode.Unauthenticated,
        Forbidden => StatusCode.PermissionDenied,
        Conflict => StatusCode.FailedPrecondition,
        _ => StatusCode.Unknown,
    };
}
