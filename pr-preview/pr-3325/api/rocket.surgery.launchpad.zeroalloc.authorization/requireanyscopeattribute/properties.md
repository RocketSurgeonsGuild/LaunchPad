# RequireAnyScopeAttribute Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequireAnyScopeAttribute](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requireanyscopeattribute.md)
- Kind: `Properties`
- Members: `1`

On a `[Policy]` class: scopes of which the policy requires the caller to have at least one. See [RequireScopeAttribute](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requirescopeattribute.md).

<a id="scopes"></a>

## Scopes

- Name: `Scopes`
- Modifiers: `get`
- Returns: `string[]`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.1b696446ea1fb50dee983bcac42661335aad175b/src/ZeroAlloc.Authorization/ScopeAttributes.cs#L31)

The scopes, any one of which is enough.

```csharp
public string[] Scopes { get; }
```
