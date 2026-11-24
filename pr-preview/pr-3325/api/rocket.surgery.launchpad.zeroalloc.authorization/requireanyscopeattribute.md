# RequireAnyScopeAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1304`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/ScopeAttributes.cs)
- Inherits: `Attribute`

On a `[Policy]` class: scopes of which the policy requires the caller to have at least one. See [RequireScopeAttribute](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requirescopeattribute.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public sealed class RequireAnyScopeAttribute
    : System.Attribute
{
    // ...
}
```

## Constructors

- [RequireAnyScopeAttribute(string[])](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requireanyscopeattribute/constructors.md#constructor-string) -- On a `[Policy]` class: scopes of which the policy requires the caller to have at least one. See [RequireScopeAttribute](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requirescopeattribute.md).

## Properties

- [Scopes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requireanyscopeattribute/properties.md#scopes) : `string[]` `get` -- The scopes, any one of which is enough.
