using System.Text.Json;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Rocket.Surgery.LaunchPad.Grpc;

/// <summary>
///     A shared interceptor for handling problem details exceptions
/// </summary>
/// <typeparam name="T"></typeparam>
[PublicAPI]
public abstract class ProblemDetailsInterceptor<T> : Interceptor
    where T : Exception, IProblemDetailsData
{
    private static Metadata CreateMetadata(T exception)
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
            metadata.Add(item.Key, item.Value is string s ? s : JsonSerializer.Serialize(item.Value));
        }

        return metadata;
    }

    private readonly StatusCode _statusCode;

    /// <summary>
    ///     Create the interceptor with it's status code
    /// </summary>
    /// <param name="statusCode"></param>
    protected ProblemDetailsInterceptor(StatusCode statusCode)
    {
        _statusCode = statusCode;
    }

    private RpcException CreateException(T exception)
    {
        return new RpcException(
            new Status(_statusCode, exception.Title ?? exception.Message, exception),
            CustomizeMetadata(CreateMetadata(exception)),
            exception.Message
        );
    }

    /// <summary>
    ///     Allows customizing the metadata before it is returned to the rpc system
    /// </summary>
    /// <param name="metadata"></param>
    /// <returns></returns>
    protected virtual Metadata CustomizeMetadata(Metadata metadata)
    {
        return metadata;
    }

    /// <inheritdoc />
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
        catch (T exception)
        {
            throw CreateException(exception);
        }
    }

    /// <inheritdoc />
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
        catch (T exception)
        {
            throw CreateException(exception);
        }
    }

    /// <inheritdoc />
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
        catch (T exception)
        {
            throw CreateException(exception);
        }
    }
}
