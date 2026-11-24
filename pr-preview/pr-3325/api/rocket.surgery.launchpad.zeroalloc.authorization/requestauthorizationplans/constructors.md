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
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L145-L147)

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
