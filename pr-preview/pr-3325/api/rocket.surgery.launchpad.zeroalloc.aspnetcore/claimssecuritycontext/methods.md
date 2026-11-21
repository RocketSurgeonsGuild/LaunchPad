# ClaimsSecurityContext Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Type: [ClaimsSecurityContext](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/claimssecuritycontext.md)
- Kind: `Methods`
- Members: `1`

A `Claims.ClaimsPrincipal` as a security context.

<a id="from"></a>
<a id="from-claimsprincipal"></a>

## From(ClaimsPrincipal?)

- Name: `From(ClaimsPrincipal?)`
- Modifiers: `static`
- Returns: `ISecurityContext`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.AspNetCore/Authorization/HttpSecurityContext.cs#L64-L88)

The principal as a security context: its subject as the id, its role claims as roles, and its claims by type, the values of a repeated claim joined by a space. A principal that is not authenticated is the anonymous security context.

```csharp
public sealed class ClaimsSecurityContext
{
    public static ISecurityContext From(
        ClaimsPrincipal? user)
    {
        // ...
    }
}
```

## Parameters

- `user` (`ClaimsPrincipal?`)
  The principal.
