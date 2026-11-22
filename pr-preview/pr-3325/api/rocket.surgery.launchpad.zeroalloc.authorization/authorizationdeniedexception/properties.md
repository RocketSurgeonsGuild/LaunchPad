# AuthorizationDeniedException Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [AuthorizationDeniedException](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdeniedexception.md)
- Kind: `Properties`
- Members: `1`

A request was denied where there is no response to answer with: a stream request, or a response that has no case for the denial. It carries the `Foundation.Unauthorized` or `Foundation.Forbidden` problem, which each transport writes as that case.

<a id="problem"></a>

## Problem

- Name: `Problem`
- Modifiers: `get`
- Returns: `Problem`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b12891a76083b718e1ed50b524f1c30a2c5447d7/src/ZeroAlloc.Authorization/AuthorizationDeniedException.cs#L12)

The `Foundation.Unauthorized` or `Foundation.Forbidden` problem.

```csharp
public Problem Problem { get; }
```
