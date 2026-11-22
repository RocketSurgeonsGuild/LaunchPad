# PolicyRequirement

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1294`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b12891a76083b718e1ed50b524f1c30a2c5447d7/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs)

One policy a request names: its name and arguments, the scopes it declares and how to run it.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public sealed class PolicyRequirement
{
    // ...
}
```

## Remarks

Describes one policy a request names.

## Constructors

- [PolicyRequirement(string, string, Type, string[], PolicyEvaluator)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement/constructors.md#constructor-string-string-type-string-policyevaluator) -- One policy a request names: its name and arguments, the scopes it declares and how to run it.

## Properties

- [Evaluate](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement/properties.md#evaluate) : [PolicyEvaluator](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyevaluator.md) `get` -- Runs the policy.
- [Key](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement/properties.md#key) : `string` `get` -- The name with the arguments: what an outcome is recorded under.
- [Name](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement/properties.md#name) : `string` `get` -- The policy's `[Policy]` name.
- [PolicyType](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement/properties.md#policytype) : `Type` `get` -- The policy class.
- [Scopes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement/properties.md#scopes) : `IReadOnlyList<string>` `get` -- The scopes the policy declares.
