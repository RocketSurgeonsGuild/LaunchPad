# ScopeRequirement

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1304`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/ScopeRequirements.cs)

One scope attribute of a policy: scopes the caller must have all of, or any one of.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public sealed class ScopeRequirement
{
    // ...
}
```

## Constructors

- [ScopeRequirement(bool, string[])](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/scoperequirement/constructors.md#constructor-bool-string) -- One scope attribute of a policy: scopes the caller must have all of, or any one of.

## Properties

- [Any](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/scoperequirement/properties.md#any) : `bool` `get` -- Whether any one of the scopes is enough; otherwise the caller must have them all.
- [Scopes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/scoperequirement/properties.md#scopes) : `IReadOnlyList<string>` `get` -- The scopes.
