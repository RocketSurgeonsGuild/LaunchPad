# AuthorizationDecision Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [AuthorizationDecision](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision.md)
- Kind: `Properties`
- Members: `4`

What a request's policies decide for a caller.

<a id="allowed"></a>

## Allowed

- Name: `Allowed`
- Modifiers: `static` `get`
- Returns: [AuthorizationDecision](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizer.cs#L17)

The caller is allowed.

```csharp
public static AuthorizationDecision Allowed { get; }
```

<a id="isallowed"></a>

## IsAllowed

- Name: `IsAllowed`
- Modifiers: `get`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizer.cs#L20)

The caller is allowed.

```csharp
public bool IsAllowed { get; }
```

<a id="problem"></a>

## Problem

- Name: `Problem`
- Modifiers: `nullable` `get; init`
- Returns: `Problem?`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizer.cs)

Null when allowed; otherwise `Foundation.Unauthorized` or `Foundation.Forbidden`.

```csharp
public Problem? Problem { get; init; }
```

<a id="requiredscopes"></a>

## RequiredScopes

- Name: `RequiredScopes`
- Modifiers: `get; init`
- Returns: `IReadOnlyList<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/RequestAuthorizer.cs)

The scopes of the requirement that denied the caller, for the challenge; empty otherwise.

```csharp
public IReadOnlyList<string> RequiredScopes { get; init; }
```
