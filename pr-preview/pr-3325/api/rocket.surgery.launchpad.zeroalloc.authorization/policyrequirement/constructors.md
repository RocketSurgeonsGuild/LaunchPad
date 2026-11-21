# PolicyRequirement Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [PolicyRequirement](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement.md)
- Kind: `Constructors`
- Members: `1`

One policy a request names: its name and arguments, the scopes it declares and how to run it.

<a id="constructor"></a>
<a id="constructor-string-string-type-string-policyevaluator"></a>

## PolicyRequirement(string, string, Type, string[], PolicyEvaluator)

- Name: `Constructor(string, string, Type, string[], PolicyEvaluator)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L26-L42)

One policy a request names: its name and arguments, the scopes it declares and how to run it.

```csharp
public sealed class PolicyRequirement
{
    public PolicyRequirement(
        string name,
        string arguments,
        Type policyType,
        string[] scopes,
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
- `scopes` (`string[]`)
  The scopes the policy declares with [RequiresScopesAttribute](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requiresscopesattribute.md).
- `evaluate` ([PolicyEvaluator](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyevaluator.md))
  Runs the policy.

## Remarks

Describes one policy a request names.
