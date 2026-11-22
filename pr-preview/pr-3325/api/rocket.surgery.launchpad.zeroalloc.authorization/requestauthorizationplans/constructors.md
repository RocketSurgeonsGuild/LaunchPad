# RequestAuthorizationPlans Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequestAuthorizationPlans](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplans.md)
- Kind: `Constructors`
- Members: `1`

The plans of the application, by request, and the policies they name, by the name an endpoint carries. One for the application.

<a id="constructor"></a>
<a id="constructor-ienumerable-requestauthorizationplan"></a>

## RequestAuthorizationPlans(IEnumerable<RequestAuthorizationPlan>)

- Name: `Constructor(IEnumerable<RequestAuthorizationPlan>)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.cda885391825d90598517a0aef31b3494df7c9f7/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L138-L140)

The plans of the application, by request, and the policies they name, by the name an endpoint carries. One for the application.

```csharp
public sealed class RequestAuthorizationPlans
{
    public RequestAuthorizationPlans(
        IEnumerable<RequestAuthorizationPlan> plans)
    {
        // ...
    }
}
```

## Parameters

- `plans` ([IEnumerable<RequestAuthorizationPlan>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan.md))
  Every plan that is registered.
