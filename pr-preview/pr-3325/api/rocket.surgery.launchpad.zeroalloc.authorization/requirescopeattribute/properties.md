# RequireScopeAttribute Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequireScopeAttribute](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requirescopeattribute.md)
- Kind: `Properties`
- Members: `1`

On a `[Policy]` class: scopes the policy requires of the caller, all of them. The policy is then what its scope attributes say and nothing else: LaunchPad writes its `EvaluateAsync`, which checks the caller's scopes, and states the scopes as those of every operation whose request names the policy.

<a id="scopes"></a>

## Scopes

- Name: `Scopes`
- Modifiers: `get`
- Returns: `string[]`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.1b696446ea1fb50dee983bcac42661335aad175b/src/ZeroAlloc.Authorization/ScopeAttributes.cs#L18)

The scopes, all of which the caller must have.

```csharp
public string[] Scopes { get; }
```
