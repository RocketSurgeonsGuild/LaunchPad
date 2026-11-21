# PolicyGroup Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [PolicyGroup](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policygroup.md)
- Kind: `Properties`
- Members: `1`

One requirement of a request: a single policy (a `[RequirePolicy]`), or several of which any one is enough (a `[RequireAnyPolicy]`). Every group of a request must pass.

<a id="policies"></a>

## Policies

- Name: `Policies`
- Modifiers: `get`
- Returns: [IReadOnlyList<PolicyRequirement>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L54)

The policies, any one of which is enough.

```csharp
public IReadOnlyList<PolicyRequirement> Policies { get; }
```
