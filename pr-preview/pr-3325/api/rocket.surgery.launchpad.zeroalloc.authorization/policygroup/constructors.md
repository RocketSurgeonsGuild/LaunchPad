# PolicyGroup Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [PolicyGroup](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policygroup.md)
- Kind: `Constructors`
- Members: `1`

One requirement of a request: a single policy (a `[RequirePolicy]`), or several of which any one is enough (a `[RequireAnyPolicy]`). Every group of a request must pass.

<a id="constructor"></a>
<a id="constructor-policyrequirement"></a>

## PolicyGroup(PolicyRequirement[])

- Name: `Constructor(PolicyRequirement[])`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.cda885391825d90598517a0aef31b3494df7c9f7/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L49-L52)

One requirement of a request: a single policy (a `[RequirePolicy]`), or several of which any one is enough (a `[RequireAnyPolicy]`). Every group of a request must pass.

```csharp
public sealed class PolicyGroup
{
    public PolicyGroup(
        params PolicyRequirement[] policies)
    {
        // ...
    }
}
```

## Parameters

- `policies` (`PolicyRequirement[]`)
  The policies, any one of which is enough.
