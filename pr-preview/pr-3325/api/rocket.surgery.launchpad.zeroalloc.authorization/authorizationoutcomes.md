# AuthorizationOutcomes

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1286`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/AuthorizationOutcomes.cs)

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
