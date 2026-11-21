# PolicyEvaluator Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [PolicyEvaluator](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyevaluator.md)
- Kind: `Methods`
- Members: `2`

Runs one policy for a caller, with the services of the call.

<a id="begininvoke"></a>
<a id="begininvoke-iserviceprovider-isecuritycontext-cancellationtoken-asynccallback-object"></a>

## BeginInvoke(IServiceProvider, ISecurityContext, CancellationToken, AsyncCallback, object)

- Name: `BeginInvoke(IServiceProvider, ISecurityContext, CancellationToken, AsyncCallback, object)`
- Modifiers: `virtual`
- Returns: `IAsyncResult`

```csharp
public delegate PolicyEvaluator
{
    public virtual IAsyncResult BeginInvoke(
        IServiceProvider services,
        ISecurityContext context,
        CancellationToken cancellationToken,
        AsyncCallback callback,
        object @object)
    {
        // ...
    }
}
```

## Parameters

- `services` (`IServiceProvider`)
- `context` (`ISecurityContext`)
- `cancellationToken` (`CancellationToken`)
- `callback` (`AsyncCallback`)
- `object` (`object`)

<a id="endinvoke"></a>
<a id="endinvoke-iasyncresult"></a>

## EndInvoke(IAsyncResult)

- Name: `EndInvoke(IAsyncResult)`
- Modifiers: `virtual`
- Returns: `ValueTask<UnitResult<AuthorizationFailure>>`

```csharp
public delegate PolicyEvaluator
{
    public virtual ValueTask<UnitResult<AuthorizationFailure>> EndInvoke(
        IAsyncResult result)
    {
        // ...
    }
}
```

## Parameters

- `result` (`IAsyncResult`)
