# ProblemDetailsInterceptor<T> Methods

- Package: [Rocket.Surgery.LaunchPad.Grpc](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.grpc.md)
- Type: [ProblemDetailsInterceptor<T>](/LaunchPad/pr-preview/pr-3173/api/rocket.surgery.launchpad.grpc/problemdetailsinterceptor-1.md)
- Kind: `Methods`
- Members: `3`

A shared interceptor for handling problem details exceptions

<a id="asyncunarycall"></a>
<a id="asyncunarycall-trequest-clientinterceptorcontext-trequest-tresponse-asyncunarycallcontinuation-trequest-tresponse"></a>

## AsyncUnaryCall(TRequest, ClientInterceptorContext<TRequest, TResponse>, AsyncUnaryCallContinuation<TRequest, TResponse>)

- Name: `AsyncUnaryCall(TRequest, ClientInterceptorContext<TRequest, TResponse>, AsyncUnaryCallContinuation<TRequest, TResponse>)`
- Modifiers: `override`
- Returns: `AsyncUnaryCall<TResponse>`

Intercepts an asynchronous invocation of a simple remote call.

```csharp
public abstract class ProblemDetailsInterceptor<T>
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        // ...
    }
}
```

## Parameters

- `request` (`TRequest`)
  The request message of the invocation.
- `context` (`ClientInterceptorContext<TRequest, TResponse>`)
  The `Interceptors.ClientInterceptorContext`2` associated with the current invocation.
- `continuation` (`AsyncUnaryCallContinuation<TRequest, TResponse>`)
  The callback that continues the invocation process. This can be invoked zero or more times by the interceptor. The interceptor can invoke the continuation passing the given request value and context arguments, or substitute them as it sees fit.

## Returns

`AsyncUnaryCall<TResponse>` -- An instance of `Core.AsyncUnaryCall`1` representing an asynchronous unary invocation. The interceptor can simply return the return value of the continuation delegate passed to it intact, or construct its own substitute as it sees fit.

<a id="blockingunarycall"></a>
<a id="blockingunarycall-trequest-clientinterceptorcontext-trequest-tresponse-blockingunarycallcontinuation-trequest-tresponse"></a>

## BlockingUnaryCall(TRequest, ClientInterceptorContext<TRequest, TResponse>, BlockingUnaryCallContinuation<TRequest, TResponse>)

- Name: `BlockingUnaryCall(TRequest, ClientInterceptorContext<TRequest, TResponse>, BlockingUnaryCallContinuation<TRequest, TResponse>)`
- Modifiers: `override`
- Returns: `TResponse`

Intercepts a blocking invocation of a simple remote call.

```csharp
public abstract class ProblemDetailsInterceptor<T>
{
    public override TResponse BlockingUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        // ...
    }
}
```

## Parameters

- `request` (`TRequest`)
  The request message of the invocation.
- `context` (`ClientInterceptorContext<TRequest, TResponse>`)
  The `Interceptors.ClientInterceptorContext`2` associated with the current invocation.
- `continuation` (`BlockingUnaryCallContinuation<TRequest, TResponse>`)
  The callback that continues the invocation process. This can be invoked zero or more times by the interceptor. The interceptor can invoke the continuation passing the given request value and context arguments, or substitute them as it sees fit.

## Returns

`TResponse` -- The response message of the current invocation. The interceptor can simply return the return value of the continuation delegate passed to it intact, or an arbitrary value as it sees fit.

<a id="unaryserverhandler"></a>
<a id="unaryserverhandler-trequest-servercallcontext-unaryservermethod-trequest-tresponse"></a>

## UnaryServerHandler(TRequest, ServerCallContext, UnaryServerMethod<TRequest, TResponse>)

- Name: `UnaryServerHandler(TRequest, ServerCallContext, UnaryServerMethod<TRequest, TResponse>)`
- Modifiers: `override`
- Returns: `Task<TResponse>`

Server-side handler for intercepting an incoming unary call.

```csharp
public abstract class ProblemDetailsInterceptor<T>
{
    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        // ...
    }
}
```

## Parameters

- `request` (`TRequest`)
  The request value of the incoming invocation.
- `context` (`ServerCallContext`)
  An instance of `Core.ServerCallContext` representing the context of the invocation.
- `continuation` (`UnaryServerMethod<TRequest, TResponse>`)
  A delegate that asynchronously proceeds with the invocation, calling the next interceptor in the chain, or the service request handler, in case of the last interceptor and return the response value of the RPC. The interceptor can choose to call it zero or more times at its discretion.

## Returns

`Task<TResponse>` -- A future representing the response value of the RPC. The interceptor can simply return the return value from the continuation intact, or an arbitrary response value as it sees fit.
