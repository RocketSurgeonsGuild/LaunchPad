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
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.AspNetCore/Authorization/HttpSecurityContext.cs#L71-L96)

The principal as a security context: its subject as the id, its role claims as roles, and its claims by type, the values of a repeated claim joined by a space. Its scopes are its `scope` claim, whichever claim the token carried them in. A principal that is not authenticated is the anonymous security context.

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
