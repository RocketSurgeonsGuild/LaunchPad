# PolicyRequirement

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1304`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs)

One policy a request names: its name and arguments, the scopes it requires and how to run it.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public sealed class PolicyRequirement
{
    // ...
}
```

## Constructors

- [PolicyRequirement(string, string, Type, string[][], PolicyEvaluator)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement/constructors.md#constructor-string-string-type-string-policyevaluator) -- One policy a request names: its name and arguments, the scopes it requires and how to run it.

## Properties

- [Evaluate](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement/properties.md#evaluate) : [PolicyEvaluator](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyevaluator.md) `get` -- Runs the policy.
- [Key](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement/properties.md#key) : `string` `get` -- The name with the arguments: what an endpoint names the policy by, and what an outcome is recorded under.
- [Name](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement/properties.md#name) : `string` `get` -- The policy's `[Policy]` name.
- [PolicyType](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement/properties.md#policytype) : `Type` `get` -- The policy class.
- [ScopeAlternatives](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement/properties.md#scopealternatives) : `IReadOnlyList<IReadOnlyList<string>>` `get` -- The scopes that satisfy the policy, as alternatives: any one set is enough. For publishing: the policy itself checks them.
- [Scopes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement/properties.md#scopes) : `IReadOnlyList<string>` `get` -- The scopes of the first alternative: what a challenge asks for.
