# ScopeRequirement Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [ScopeRequirement](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/scoperequirement.md)
- Kind: `Properties`
- Members: `2`

One scope attribute of a policy: scopes the caller must have all of, or any one of.

<a id="any"></a>

## Any

- Name: `Any`
- Modifiers: `get`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.1b696446ea1fb50dee983bcac42661335aad175b/src/ZeroAlloc.Authorization/ScopeRequirements.cs#L13)

Whether any one of the scopes is enough; otherwise the caller must have them all.

```csharp
public bool Any { get; }
```

<a id="scopes"></a>

## Scopes

- Name: `Scopes`
- Modifiers: `get`
- Returns: `IReadOnlyList<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.1b696446ea1fb50dee983bcac42661335aad175b/src/ZeroAlloc.Authorization/ScopeRequirements.cs#L16)

The scopes.

```csharp
public IReadOnlyList<string> Scopes { get; }
```
