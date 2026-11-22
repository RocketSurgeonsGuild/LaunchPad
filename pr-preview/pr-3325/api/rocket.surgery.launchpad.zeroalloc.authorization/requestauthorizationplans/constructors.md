# RequestAuthorizationPlans Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequestAuthorizationPlans](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplans.md)
- Kind: `Constructors`
- Members: `1`

The plans of the application, by the name the edge knows them by, and what the edge has learned about its policies. One for the application.

<a id="constructor"></a>
<a id="constructor-ienumerable-requestauthorizationplan"></a>

## RequestAuthorizationPlans(IEnumerable<RequestAuthorizationPlan>)

- Name: `Constructor(IEnumerable<RequestAuthorizationPlan>)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L176-L181)

The plans of the application, by the name the edge knows them by, and what the edge has learned about its policies. One for the application.

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
