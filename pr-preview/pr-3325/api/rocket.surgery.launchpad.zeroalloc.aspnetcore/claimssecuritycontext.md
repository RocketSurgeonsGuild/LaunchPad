# ClaimsSecurityContext

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Version: `2.0.1-pr.1298`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.1b696446ea1fb50dee983bcac42661335aad175b/src/ZeroAlloc.AspNetCore/Authorization/HttpSecurityContext.cs)
- Implements: `ISecurityContext`

A `Claims.ClaimsPrincipal` as a security context.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Authorization;

public sealed class ClaimsSecurityContext
    : ZeroAlloc.Authorization.ISecurityContext
{
    // ...
}
```

## Properties

- [Claims](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/claimssecuritycontext/properties.md#claims) : `IReadOnlyDictionary<string, string>` `get`
- [Id](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/claimssecuritycontext/properties.md#id) : `string` `get`
- [Roles](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/claimssecuritycontext/properties.md#roles) : `IReadOnlySet<string>` `get`

## Methods

- [From(ClaimsPrincipal?)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/claimssecuritycontext/methods.md#from-claimsprincipal) : `ISecurityContext` `static` -- The principal as a security context: its subject as the id, its role claims as roles, and its claims by type, the values of a repeated claim joined by a space. Its scopes are its `scope` claim, whichever claim the token carried them in. A principal that is not authenticated is the anonymous security context.
