# RequestAuthorizationOptions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Version: `2.0.1-pr.1294`
- Namespace: `Rocket.Surgery.LaunchPad.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b12891a76083b718e1ed50b524f1c30a2c5447d7/src/ZeroAlloc.Authorization/RequestAuthorizationOptions.cs)

How LaunchPad reads what a caller was granted.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Authorization;

public sealed class RequestAuthorizationOptions
{
    // ...
}
```

## Constructors

- [RequestAuthorizationOptions](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationoptions/constructors.md#constructor)

## Properties

- [ScopeClaims](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationoptions/properties.md#scopeclaims) : `IList<string>` `get` -- The claims of the security context that hold the caller's scopes, each a space separated list. `scope` (OAuth 2.0) and `scp` (Microsoft Entra), under that name or the claim type ASP.NET Core maps it to, unless changed.
