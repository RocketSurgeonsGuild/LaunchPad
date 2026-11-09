using Grpc.Core;
using Grpc.Core.Interceptors;

using ZeroAlloc.Mediator.Validation;

namespace Rocket.Surgery.LaunchPad.Grpc.Validation;

/// <summary>
///     Turns a <see cref="ValidationFailedException" /> into an <see cref="StatusCode.InvalidArgument" /> <see cref="RpcException" />: the detail
///     lists each failure, and the trailers carry one entry per failure, keyed by the lower-cased property name.
/// </summary>
internal sealed class ZeroAllocValidationInterceptor : Interceptor
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
        catch (ValidationFailedException exception)
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
        catch (ValidationFailedException exception)
        {
            throw ToRpcException(exception);
        }
    }

    internal static RpcException ToRpcException(ValidationFailedException exception)
    {
        var failures = exception.Error.Failures.ToArray();
        var trailers = new Metadata();
        foreach (var failure in failures)
        {
            trailers.Add(failure.PropertyName.ToLowerInvariant(), failure.ErrorMessage);
        }

        var detail = string.Join(Environment.NewLine, failures.Select(failure => $"Property {failure.PropertyName} failed validation. {failure.ErrorMessage}"));
        return new(new(StatusCode.InvalidArgument, detail), trailers);
    }
}
