using Grpc.Core;
using Grpc.Core.Interceptors;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Rocket.Surgery.LaunchPad.Grpc;

internal class RequestFailedInterceptor : Interceptor
{
    private static RpcException CreateException(RequestFailedException exception)
    {
        return new RpcException(
            new Status(StatusCode.FailedPrecondition, exception.Title, exception),
            CreateMetadata(exception),
            exception.Message
        );
    }

    private static Metadata CreateMetadata(RequestFailedException exception)
    {
        var metadata = new Metadata();
        if (exception.Title is { })
            metadata.Add("title", exception.Title);
        if (exception.Instance is { })
            metadata.Add("instance", exception.Instance);
        if (exception.Link is { })
            metadata.Add("link", exception.Link);
        metadata.Add("message", exception.Message);
        foreach (var item in exception.Properties)
        {
            metadata.Add(item.Key, item.Value?.ToString());
        }

        return metadata;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation
    )
    {
        try
        {
            return await continuation(request, context);
        }
        catch (RequestFailedException exception)
        {
            throw CreateException(exception);
        }
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation
    )
    {
        try
        {
            return continuation(request, context);
        }
        catch (RequestFailedException exception)
        {
            throw CreateException(exception);
        }
    }

    public override TResponse BlockingUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        BlockingUnaryCallContinuation<TRequest, TResponse> continuation
    )
    {
        try
        {
            return continuation(request, context);
        }
        catch (RequestFailedException exception)
        {
            throw CreateException(exception);
        }
    }
}
