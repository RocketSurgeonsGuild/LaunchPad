# PolicyGroup Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [PolicyGroup](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policygroup.md)
- Kind: `Properties`
- Members: `2`

One requirement of a request: a single policy (a `[RequirePolicy]`), or several of which any one is enough (a `[RequireAnyPolicy]`). Every group of a request must pass.

<a id="policies"></a>

## Policies

- Name: `Policies`
- Modifiers: `get`
- Returns: [IReadOnlyList<PolicyRequirement>](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L56)

The policies, any one of which is enough.

```csharp
public IReadOnlyList<PolicyRequirement> Policies { get; }
```

<a id="scopealternatives"></a>

## ScopeAlternatives

- Name: `ScopeAlternatives`
- Modifiers: `get`
- Returns: `IReadOnlyList<IReadOnlyList<string>>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L59)

The scopes that satisfy the group, as alternatives: those of each of its policies.

```csharp
public IReadOnlyList<IReadOnlyList<string>> ScopeAlternatives { get; }
```
