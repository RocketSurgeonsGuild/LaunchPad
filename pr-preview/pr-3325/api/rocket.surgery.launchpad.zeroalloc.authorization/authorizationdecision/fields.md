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
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.cda885391825d90598517a0aef31b3494df7c9f7/src/ZeroAlloc.Authorization/RequestAuthorizer.cs)

The key of `Problem.Properties` that holds the scopes a forbidden caller lacks, separated by spaces.

```csharp
public const static string MissingScopesProperty
```
