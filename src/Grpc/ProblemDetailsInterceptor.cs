using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using Grpc.Core;
using Grpc.Core.Interceptors;

using Rocket.Surgery.LaunchPad.Foundation;

namespace Rocket.Surgery.LaunchPad.Grpc;

/// <summary>
///     A shared interceptor for handling problem details exceptions
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
///     Create the interceptor with its status code
/// </remarks>
/// <param name="statusCode"></param>
/// <param name="options">
///     The JSON options the problem details properties are written with. Their resolver chain must know the property types: the built in
///     interceptors use the ASP.NET Core HTTP JSON options, which LaunchPad's <c>SystemTextJsonJoint</c> fills from the registered resolvers.
/// </param>
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
                item.Value switch { string s => s, { } o => Serialize(o, options), _ => null } ?? ""
            );
        }

        return metadata;
    }

    // Written with the metadata the options' resolver chain has for the value's type, so no reflection is needed; a value no resolver knows is
    // sent as its ToString().
    private static string? Serialize(object value, JsonSerializerOptions options)
    {
        JsonTypeInfo? typeInfo;
        try
        {
            typeInfo = options.TryGetTypeInfo(value.GetType(), out var resolved) ? resolved : null;
        }
        catch (NotSupportedException)
        {
            typeInfo = null;
        }
        catch (InvalidOperationException)
        {
            typeInfo = null;
        }

        return typeInfo is null ? value.ToString() : JsonSerializer.Serialize(value, typeInfo);
    }

    private RpcException CreateException(T exception, JsonSerializerOptions options) => new(
        new(_statusCode, exception.Title ?? exception.Message, exception),
        CustomizeMetadata(CreateMetadata(exception, options)),
        exception.Message
    );

    private readonly StatusCode _statusCode = statusCode;
    private readonly JsonSerializerOptions _options = options;
}
