# ScopeRequirements Fields

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [ScopeRequirements](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/scoperequirements.md)
- Kind: `Fields`
- Members: `2`

Checks the scopes of a caller: what the `EvaluateAsync` LaunchPad writes for a policy with scope attributes calls. A policy is checked wherever its request is dispatched, with or without a transport, so the check is the policy's own.

<a id="missingscopecode"></a>

## MissingScopeCode

- Name: `MissingScopeCode`
- Modifiers: `static` `const`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/ScopeRequirements.cs)

The code of the failure a caller without a required scope is denied with. Its reason is the scopes the caller lacks, separated by spaces.

```csharp
public const static string MissingScopeCode
```

<a id="scopeclaim"></a>

## ScopeClaim

- Name: `ScopeClaim`
- Modifiers: `static` `const`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/ScopeRequirements.cs)

The claim of the security context that holds the caller's scopes, separated by spaces.

```csharp
public const static string ScopeClaim
```
