# PolicyRequirement Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [PolicyRequirement](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement.md)
- Kind: `Constructors`
- Members: `1`

One policy a request names: its name and arguments, the scopes it requires and how to run it.

<a id="constructor"></a>
<a id="constructor-string-string-type-string-policyevaluator"></a>

## PolicyRequirement(string, string, Type, string[][], PolicyEvaluator)

- Name: `Constructor(string, string, Type, string[][], PolicyEvaluator)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L26-L44)

One policy a request names: its name and arguments, the scopes it requires and how to run it.

```csharp
public sealed class PolicyRequirement
{
    public PolicyRequirement(
        string name,
        string arguments,
        Type policyType,
        string[][] scopeAlternatives,
        PolicyEvaluator evaluate)
    {
        // ...
    }
}
```

## Parameters

- `name` (`string`)
  The policy's `[Policy]` name.
- `arguments` (`string`)
  The constant arguments the request passes it, as text; empty for none.
- `policyType` (`Type`)
  The policy class.
- `scopeAlternatives` (`string[][]`)
  The scopes the policy's scope attributes require, as alternatives: each a set of scopes that satisfies the policy. One empty set for a policy without scope attributes.
- `evaluate` ([PolicyEvaluator](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyevaluator.md))
  Runs the policy.
