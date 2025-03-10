using System.Text.Json;

using Grpc.Core;
using Grpc.Core.Interceptors;

using Rocket.Surgery.LaunchPad.Primitives;

namespace Rocket.Surgery.LaunchPad.Grpc;

/// <summary>
///     A shared interceptor for handling problem details exceptions
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
///     Create the interceptor with its status code
/// </remarks>
/// <param name="statusCode"></param>
/// <param name="options"></param>
[PublicAPI]
public abstract class ProblemDetailsInterceptor<T>(StatusCode statusCode, JsonSerializerOptions options) : Interceptor
    where T : Exception, IProblemDetailsData
{
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
            throw CreateException(exception, _options);
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
            throw CreateException(exception, _options);
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
            throw CreateException(exception, _options);
        }
    }

    /// <summary>
    ///     Allows customizing the metadata before it is returned to the rpc system
    /// </summary>
    /// <param name="metadata"></param>
    /// <returns></returns>
    protected virtual Metadata CustomizeMetadata(Metadata metadata) => metadata;

    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = "options is assumed to have a proper resolver chain"
    )]
    private static Metadata CreateMetadata(T exception, JsonSerializerOptions options)
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
            metadata.Add(
                item.Key,
                item.Value switch { string s => s, { } o => JsonSerializer.Serialize(o, o.GetType(), options), _ => null } ?? ""
            );
        }

        return metadata;
    }

    private RpcException CreateException(T exception, JsonSerializerOptions options) => new(
        new(_statusCode, exception.Title ?? exception.Message, exception),
        CustomizeMetadata(CreateMetadata(exception, options)),
        exception.Message
    );

    private readonly StatusCode _statusCode = statusCode;
    private readonly JsonSerializerOptions _options = options;
}
