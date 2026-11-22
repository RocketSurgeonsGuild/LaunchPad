# RequiresScopesAttribute Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequiresScopesAttribute](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requiresscopesattribute.md)
- Kind: `Properties`
- Members: `1`

On a `[Policy]` class: the scopes the policy requires of the caller, all of them. LaunchPad checks them before the policy runs, and publishes them as the scopes of every operation whose request names the policy.

<a id="scopes"></a>

## Scopes

- Name: `Scopes`
- Modifiers: `get`
- Returns: `string[]`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.cda885391825d90598517a0aef31b3494df7c9f7/src/ZeroAlloc.Authorization/RequiresScopesAttribute.cs#L13)

The scopes, as they appear in the caller's token.

```csharp
public string[] Scopes { get; }
```
