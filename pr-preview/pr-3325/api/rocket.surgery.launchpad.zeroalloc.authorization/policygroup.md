# PolicyGroup

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1304`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs)

One requirement of a request: a single policy (a `[RequirePolicy]`), or several of which any one is enough (a `[RequireAnyPolicy]`). Every group of a request must pass.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public sealed class PolicyGroup
{
    // ...
}
```

## Constructors

- [PolicyGroup(PolicyRequirement[])](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policygroup/constructors.md#constructor-policyrequirement) -- One requirement of a request: a single policy (a `[RequirePolicy]`), or several of which any one is enough (a `[RequireAnyPolicy]`). Every group of a request must pass.

## Properties

- [Policies](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policygroup/properties.md#policies) : [IReadOnlyList<PolicyRequirement>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement.md) `get` -- The policies, any one of which is enough.
- [ScopeAlternatives](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policygroup/properties.md#scopealternatives) : `IReadOnlyList<IReadOnlyList<string>>` `get` -- The scopes that satisfy the group, as alternatives: those of each of its policies.
