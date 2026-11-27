using Grpc.Core;
using Grpc.Core.Interceptors;

using Rocket.Surgery.LaunchPad.Authorization;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Rocket.Surgery.LaunchPad.Grpc.Authorization;

/// <summary>
///     Turns an <see cref="AuthorizationDeniedException" /> (the denial of a stream request, which has no response to answer with) into the
///     status of its case, <see cref="StatusCode.Unauthenticated" /> or <see cref="StatusCode.PermissionDenied" />, with the case name in the
///     <c>case</c> trailer, as the generated gRPC servers answer a union case.
/// </summary>
internal sealed class AuthorizationDeniedInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation
    )
    {
        try
        {
            return await continuation(request, context).ConfigureAwait(false);
        }
        catch (AuthorizationDeniedException exception)
        {
            throw ToRpcException(exception);
        }
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation
    )
    {
        try
        {
            await continuation(request, responseStream, context).ConfigureAwait(false);
        }
        catch (AuthorizationDeniedException exception)
        {
            throw ToRpcException(exception);
        }
    }

    internal static RpcException ToRpcException(AuthorizationDeniedException exception)
    {
        var problem = exception.Problem;
        var trailers = new Metadata { { "case", problem.GetType().Name } };
        return new(new(problem is Unauthorized ? StatusCode.Unauthenticated : StatusCode.PermissionDenied, problem.Detail ?? problem.GetType().Name), trailers);
    }
}
