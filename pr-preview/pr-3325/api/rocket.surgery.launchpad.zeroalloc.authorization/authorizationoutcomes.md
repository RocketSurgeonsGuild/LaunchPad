# AuthorizationOutcomes

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1304`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/AuthorizationOutcomes.cs)

What each policy decided for the current call, by policy name and arguments. The edge and the mediator pipeline share it through the call's service scope, so a policy runs once.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public sealed class AuthorizationOutcomes
{
    // ...
}
```

## Constructors

- [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes/constructors.md#constructor)

## Methods

- [Record(PolicyRequirement, PolicyOutcome)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes/methods.md#record-policyrequirement-policyoutcome) -- Records what a policy decided for this call.
- [TryGet(PolicyRequirement, PolicyOutcome)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes/methods.md#tryget-policyrequirement-policyoutcome) : `bool` -- The outcome recorded for a policy, if it has run for this call.
