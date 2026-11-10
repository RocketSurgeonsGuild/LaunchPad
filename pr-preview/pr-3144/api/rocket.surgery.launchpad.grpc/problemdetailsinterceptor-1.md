# ProblemDetailsInterceptor<T>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Grpc](/LaunchPad/pr-preview/pr-3144/api/rocket.surgery.launchpad.grpc.md)
- Version: `2.0.1-pr.1165`
- Namespace: `Rocket.Surgery.LaunchPad.Grpc`
- Target framework: `net10.0`
- Inherits: `Interceptor`

A shared interceptor for handling problem details exceptions

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Grpc;

public abstract class ProblemDetailsInterceptor<T>
    : Grpc.Core.Interceptors.Interceptor
    where T : Exception, IProblemDetailsData
{
    // ...
}
```

## Remarks

Create the interceptor with its status code

## Methods

- [AsyncUnaryCall(TRequest, ClientInterceptorContext<TRequest, TResponse>, AsyncUnaryCallContinuation<TRequest, TResponse>)](/LaunchPad/pr-preview/pr-3144/api/rocket.surgery.launchpad.grpc/problemdetailsinterceptor-1/methods.md#asyncunarycall-trequest-clientinterceptorcontext-trequest-tresponse-asyncunarycallcontinuation-trequest-tresponse) : `AsyncUnaryCall<TResponse>` -- Intercepts an asynchronous invocation of a simple remote call.
- [BlockingUnaryCall(TRequest, ClientInterceptorContext<TRequest, TResponse>, BlockingUnaryCallContinuation<TRequest, TResponse>)](/LaunchPad/pr-preview/pr-3144/api/rocket.surgery.launchpad.grpc/problemdetailsinterceptor-1/methods.md#blockingunarycall-trequest-clientinterceptorcontext-trequest-tresponse-blockingunarycallcontinuation-trequest-tresponse) : `TResponse` -- Intercepts a blocking invocation of a simple remote call.
- [UnaryServerHandler(TRequest, ServerCallContext, UnaryServerMethod<TRequest, TResponse>)](/LaunchPad/pr-preview/pr-3144/api/rocket.surgery.launchpad.grpc/problemdetailsinterceptor-1/methods.md#unaryserverhandler-trequest-servercallcontext-unaryservermethod-trequest-tresponse) : `Task<TResponse>` -- Server-side handler for intercepting an incoming unary call.
