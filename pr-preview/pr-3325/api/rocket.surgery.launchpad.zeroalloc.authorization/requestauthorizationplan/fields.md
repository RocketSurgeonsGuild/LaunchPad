# RequestAuthorizationPlan Fields

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequestAuthorizationPlan](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan.md)
- Kind: `Fields`
- Members: `2`

The policies a request requires, written by LaunchPad's generator for every request with a policy attribute and registered when its assembly loads.

<a id="maxalternatives"></a>

## MaxAlternatives

- Name: `MaxAlternatives`
- Modifiers: `static` `const`
- Returns: `int`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs)

The most alternatives that are published.

```csharp
public const static int MaxAlternatives
```

<a id="policyprefix"></a>

## PolicyPrefix

- Name: `PolicyPrefix`
- Modifiers: `static` `const`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizationPlan.cs)

What an edge policy name starts with; the rest is the plan's [RequestAuthorizationPlan.Id](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationplan/properties.md#id).

```csharp
public const static string PolicyPrefix
```
