# ScopeRequirements

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1304`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/ScopeRequirements.cs)

Checks the scopes of a caller: what the `EvaluateAsync` LaunchPad writes for a policy with scope attributes calls. A policy is checked wherever its request is dispatched, with or without a transport, so the check is the policy's own.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public static class ScopeRequirements
{
    // ...
}
```

## Methods

- [Evaluate(ISecurityContext, ScopeRequirement[])](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/scoperequirements/methods.md#evaluate-isecuritycontext-scoperequirement) : `UnitResult<AuthorizationFailure>` `static` -- Whether the caller satisfies every requirement; otherwise a failure with [ScopeRequirements.MissingScopeCode](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/scoperequirements/fields.md#missingscopecode) that names the scopes of the first requirement the caller doesn't satisfy.
- [Granted(ISecurityContext)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/scoperequirements/methods.md#granted-isecuritycontext) : `IReadOnlySet<string>` `static` -- The scopes the caller has.

## Fields

- [MissingScopeCode](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/scoperequirements/fields.md#missingscopecode) : `string` `static` -- The code of the failure a caller without a required scope is denied with. Its reason is the scopes the caller lacks, separated by spaces.
- [ScopeClaim](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/scoperequirements/fields.md#scopeclaim) : `string` `static` -- The claim of the security context that holds the caller's scopes, separated by spaces.
