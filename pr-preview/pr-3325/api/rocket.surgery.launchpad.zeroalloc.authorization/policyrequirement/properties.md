# PolicyRequirement Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [PolicyRequirement](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement.md)
- Kind: `Properties`
- Members: `6`

One policy a request names: its name and arguments, the scopes it requires and how to run it.

<a id="evaluate"></a>

## Evaluate

- Name: `Evaluate`
- Modifiers: `get`
- Returns: [PolicyEvaluator](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyevaluator.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L44)

Runs the policy.

```csharp
public PolicyEvaluator Evaluate { get; }
```

<a id="key"></a>

## Key

- Name: `Key`
- Modifiers: `get`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L32)

The name with the arguments: what an endpoint names the policy by, and what an outcome is recorded under.

```csharp
public string Key { get; }
```

<a id="name"></a>

## Name

- Name: `Name`
- Modifiers: `get`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L29)

The policy's `[Policy]` name.

```csharp
public string Name { get; }
```

<a id="policytype"></a>

## PolicyType

- Name: `PolicyType`
- Modifiers: `get`
- Returns: `Type`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L35)

The policy class.

```csharp
public Type PolicyType { get; }
```

<a id="scopealternatives"></a>

## ScopeAlternatives

- Name: `ScopeAlternatives`
- Modifiers: `get`
- Returns: `IReadOnlyList<IReadOnlyList<string>>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L38)

The scopes that satisfy the policy, as alternatives: any one set is enough. For publishing: the policy itself checks them.

```csharp
public IReadOnlyList<IReadOnlyList<string>> ScopeAlternatives { get; }
```

<a id="scopes"></a>

## Scopes

- Name: `Scopes`
- Modifiers: `get`
- Returns: `IReadOnlyList<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L41)

The scopes of the first alternative: what a challenge asks for.

```csharp
public IReadOnlyList<string> Scopes { get; }
```
