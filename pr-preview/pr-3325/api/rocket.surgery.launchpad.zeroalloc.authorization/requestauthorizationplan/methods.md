# RequestAuthorizationPlan Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequestAuthorizationPlan](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan.md)
- Kind: `Methods`
- Members: `1`

The policies a request requires. LaunchPad's generator writes one for every request with a policy attribute, and registers it with the services of the application ( [AuthorizationServiceCollectionExtensions.AddRequestAuthorizationPlan(IServiceCollection, PolicyGroup[])](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationservicecollectionextensions/methods.md#addrequestauthorizationplan-iservicecollection-policygroup)).

<a id="attheedge"></a>
<a id="attheedge-isecuritycontext"></a>

## AtTheEdge(ISecurityContext)

- Name: `AtTheEdge(ISecurityContext)`
- Modifiers: `abstract`
- Returns: `ISecurityContext`

The caller as policies see them at the edge, where there is no request yet: reading the request throws.

```csharp
public abstract class RequestAuthorizationPlan
{
    public abstract ISecurityContext AtTheEdge(
        ISecurityContext context)
    {
        // ...
    }
}
```

## Parameters

- `context` (`ISecurityContext`)
  The caller.
