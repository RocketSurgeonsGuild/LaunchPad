# AuthorizationDecision Fields

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [AuthorizationDecision](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision.md)
- Kind: `Fields`
- Members: `1`

What a request's policies decide for a caller.

<a id="missingscopesproperty"></a>

## MissingScopesProperty

- Name: `MissingScopesProperty`
- Modifiers: `static` `const`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.1b696446ea1fb50dee983bcac42661335aad175b/src/ZeroAlloc.Authorization/RequestAuthorizer.cs)

The key of `Problem.Properties` that holds the scopes a forbidden caller lacks, separated by spaces.

```csharp
public const static string MissingScopesProperty
```
