# RequestAuthorizationPlan<TRequest> Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequestAuthorizationPlan<TRequest>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan-1.md)
- Kind: `Methods`
- Members: `2`

The plan of `TRequest`.

<a id="attheedge"></a>
<a id="attheedge-isecuritycontext"></a>

## AtTheEdge(ISecurityContext)

- Name: `AtTheEdge(ISecurityContext)`
- Modifiers: `override`
- Returns: `ISecurityContext`

The caller as policies see them at the edge, where there is no request yet: reading the request throws.

```csharp
public sealed class RequestAuthorizationPlan<TRequest>
{
    public override ISecurityContext AtTheEdge(
        ISecurityContext context)
    {
        // ...
    }
}
```

## Parameters

- `context` (`ISecurityContext`)
  The caller.

<a id="withrequest"></a>
<a id="withrequest-isecuritycontext-trequest"></a>

## WithRequest(ISecurityContext, TRequest)

- Name: `WithRequest(ISecurityContext, TRequest)`
- Returns: `ISecurityContext`

The caller as policies see them in the pipeline: with the request as the resource.

```csharp
public sealed class RequestAuthorizationPlan<TRequest>
{
    public ISecurityContext WithRequest(
        ISecurityContext context,
        TRequest request)
    {
        // ...
    }
}
```

## Parameters

- `context` (`ISecurityContext`)
  The caller.
- `request` (`TRequest`)
  The request.
