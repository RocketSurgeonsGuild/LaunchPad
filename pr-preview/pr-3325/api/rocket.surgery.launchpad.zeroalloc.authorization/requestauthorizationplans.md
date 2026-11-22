# RequestAuthorizationPlans

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1291`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs)

The plans of the application, by the name the edge knows them by, and what the edge has learned about its policies. One for the application.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public sealed class RequestAuthorizationPlans
{
    // ...
}
```

## Constructors

- [RequestAuthorizationPlans(IEnumerable<RequestAuthorizationPlan>)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplans/constructors.md#constructor-ienumerable-requestauthorizationplan) -- The plans of the application, by the name the edge knows them by, and what the edge has learned about its policies. One for the application.

## Methods

- [Find(string)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplans/methods.md#find-string) : [RequestAuthorizationPlan?](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan.md) -- The plan of the request with this id, if it has policies.
- [FindByPolicyName(string?)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplans/methods.md#findbypolicyname-string) : [RequestAuthorizationPlan?](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan.md) -- The plan an edge policy name stands for; null for any other policy name, and for a request without a plan.
