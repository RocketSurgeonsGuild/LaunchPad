# PolicyRequirement Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [PolicyRequirement](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyrequirement.md)
- Kind: `Properties`
- Members: `5`

One policy a request names: its name and arguments, the scopes it declares and how to run it.

<a id="evaluate"></a>

## Evaluate

- Name: `Evaluate`
- Modifiers: `get`
- Returns: [PolicyEvaluator](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyevaluator.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L42)

Runs the policy.

```csharp
public PolicyEvaluator Evaluate { get; }
```

<a id="key"></a>

## Key

- Name: `Key`
- Modifiers: `get`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L33)

The name with the arguments: what an outcome is recorded under.

```csharp
public string Key { get; }
```

<a id="name"></a>

## Name

- Name: `Name`
- Modifiers: `get`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L30)

The policy's `[Policy]` name.

```csharp
public string Name { get; }
```

<a id="policytype"></a>

## PolicyType

- Name: `PolicyType`
- Modifiers: `get`
- Returns: `Type`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L36)

The policy class.

```csharp
public Type PolicyType { get; }
```

<a id="scopes"></a>

## Scopes

- Name: `Scopes`
- Modifiers: `get`
- Returns: `IReadOnlyList<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs#L39)

The scopes the policy declares.

```csharp
public IReadOnlyList<string> Scopes { get; }
```
