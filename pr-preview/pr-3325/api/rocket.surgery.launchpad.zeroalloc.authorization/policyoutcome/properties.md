# PolicyOutcome Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [PolicyOutcome](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome.md)
- Kind: `Properties`
- Members: `4`

What one policy decided: allowed, or denied with the scopes the caller lacks or the policy's own failure.

<a id="allowed"></a>

## Allowed

- Name: `Allowed`
- Modifiers: `get; init`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/AuthorizationOutcomes.cs)

The policy allows the caller.

```csharp
public bool Allowed { get; init; }
```

<a id="failure"></a>

## Failure

- Name: `Failure`
- Modifiers: `nullable` `get; init`
- Returns: `AuthorizationFailure?`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/AuthorizationOutcomes.cs)

The policy's own failure, when its logic denied the caller.

```csharp
public AuthorizationFailure? Failure { get; init; }
```

<a id="missingscopes"></a>

## MissingScopes

- Name: `MissingScopes`
- Modifiers: `get; init`
- Returns: `IReadOnlyList<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/AuthorizationOutcomes.cs)

The declared scopes the caller lacks; empty when the policy's own logic decided.

```csharp
public IReadOnlyList<string> MissingScopes { get; init; }
```

<a id="success"></a>

## Success

- Name: `Success`
- Modifiers: `static` `get`
- Returns: [PolicyOutcome](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/AuthorizationOutcomes.cs#L41)

The policy allows the caller.

```csharp
public static PolicyOutcome Success { get; }
```
