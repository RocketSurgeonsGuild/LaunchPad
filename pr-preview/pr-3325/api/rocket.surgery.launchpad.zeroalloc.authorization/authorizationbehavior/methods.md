# AuthorizationBehavior Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [AuthorizationBehavior](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationbehavior.md)
- Kind: `Methods`
- Members: `3`

The ZeroAlloc.Mediator pipeline behavior that checks a request's policies before anything else runs. A caller with no identity answers the `Foundation.Unauthorized` case of the response union, and a caller a policy denies answers `Foundation.Forbidden`; a request without policies passes straight through.

<a id="checkstream"></a>
<a id="checkstream-trequest-iserviceprovider-cancellationtoken"></a>

## CheckStream(TRequest, IServiceProvider?, CancellationToken)

- Name: `CheckStream(TRequest, IServiceProvider?, CancellationToken)`
- Modifiers: `static`
- Returns: `ValueTask`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b12891a76083b718e1ed50b524f1c30a2c5447d7/src/ZeroAlloc.Authorization/AuthorizationBehavior.cs#L71-L75)

Checks a stream request's policies before its handler runs; a denial throws [AuthorizationDeniedException](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdeniedexception.md).

```csharp
public sealed class AuthorizationBehavior
{
    public static ValueTask CheckStream<TRequest>(
        TRequest request,
        IServiceProvider? services,
        CancellationToken ct)
    {
        // ...
    }
}
```

## Parameters

- `request` (`TRequest`)
  The stream request.
- `services` (`IServiceProvider?`)
  The services of the request being dispatched.
- `ct` (`CancellationToken`)
  The cancellation token of the enumeration.

<a id="handle"></a>
<a id="handle-trequest-cancellationtoken-func-trequest-cancellationtoken-valuetask-tresponse"></a>

## Handle(TRequest, CancellationToken, Func<TRequest, CancellationToken, ValueTask<TResponse>>)

- Name: `Handle(TRequest, CancellationToken, Func<TRequest, CancellationToken, ValueTask<TResponse>>)`
- Modifiers: `static`
- Returns: `ValueTask<TResponse>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b12891a76083b718e1ed50b524f1c30a2c5447d7/src/ZeroAlloc.Authorization/AuthorizationBehavior.cs#L43)

Checks the request's policies with the services of the request being dispatched.

```csharp
public sealed class AuthorizationBehavior
{
    public static ValueTask<TResponse> Handle<TRequest, TResponse>(
        TRequest request,
        CancellationToken ct,
        Func<TRequest, CancellationToken, ValueTask<TResponse>> next)
    {
        // ...
    }
}
```

## Parameters

- `request` (`TRequest`)
  The request.
- `ct` (`CancellationToken`)
  The cancellation token of the request.
- `next` (`Func<TRequest, CancellationToken, ValueTask<TResponse>>`)
  The rest of the pipeline.

<a id="handle-trequest-iserviceprovider-cancellationtoken-func-trequest-cancellationtoken-valuetask-tresponse"></a>

## Handle(TRequest, IServiceProvider?, CancellationToken, Func<TRequest, CancellationToken, ValueTask<TResponse>>)

- Name: `Handle(TRequest, IServiceProvider?, CancellationToken, Func<TRequest, CancellationToken, ValueTask<TResponse>>)`
- Modifiers: `static`
- Returns: `ValueTask<TResponse>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b12891a76083b718e1ed50b524f1c30a2c5447d7/src/ZeroAlloc.Authorization/AuthorizationBehavior.cs#L60-L61)

Checks the request's policies with the services of the request being dispatched.

```csharp
public sealed class AuthorizationBehavior
{
    public static ValueTask<TResponse> Handle<TRequest, TResponse>(
        TRequest request,
        IServiceProvider? services,
        CancellationToken ct,
        Func<TRequest, CancellationToken, ValueTask<TResponse>> next)
    {
        // ...
    }
}
```

## Parameters

- `request` (`TRequest`)
  The request.
- `services` (`IServiceProvider?`)
  The services of the request being dispatched.
- `ct` (`CancellationToken`)
  The cancellation token of the request.
- `next` (`Func<TRequest, CancellationToken, ValueTask<TResponse>>`)
  The rest of the pipeline.
