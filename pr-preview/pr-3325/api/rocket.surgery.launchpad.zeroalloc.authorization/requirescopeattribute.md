# RequireScopeAttribute

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1298`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.1b696446ea1fb50dee983bcac42661335aad175b/src/ZeroAlloc.Authorization/ScopeAttributes.cs)
- Inherits: `Attribute`

On a `[Policy]` class: scopes the policy requires of the caller, all of them. The policy is then what its scope attributes say and nothing else: LaunchPad writes its `EvaluateAsync`, which checks the caller's scopes, and states the scopes as those of every operation whose request names the policy.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public sealed class RequireScopeAttribute
    : System.Attribute
{
    // ...
}
```

## Remarks

The class is partial and lists `IAuthorizationPolicy`. Several attributes on a class must all be satisfied, alongside its [RequireAnyScopeAttribute](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requireanyscopeattribute.md) s.

## Constructors

- [RequireScopeAttribute(string[])](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requirescopeattribute/constructors.md#constructor-string) -- On a `[Policy]` class: scopes the policy requires of the caller, all of them. The policy is then what its scope attributes say and nothing else: LaunchPad writes its `EvaluateAsync`, which checks the caller's scopes, and states the scopes as those of every operation whose request names the policy.

## Properties

- [Scopes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requirescopeattribute/properties.md#scopes) : `string[]` `get` -- The scopes, all of which the caller must have.
