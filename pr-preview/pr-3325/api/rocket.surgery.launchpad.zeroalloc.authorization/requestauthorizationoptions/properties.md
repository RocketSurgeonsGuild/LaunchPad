# RequestAuthorizationOptions Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequestAuthorizationOptions](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requestauthorizationoptions.md)
- Kind: `Properties`
- Members: `1`

How LaunchPad reads what a caller was granted.

<a id="scopeclaims"></a>

## ScopeClaims

- Name: `ScopeClaims`
- Modifiers: `get`
- Returns: `IList<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.cda885391825d90598517a0aef31b3494df7c9f7/src/ZeroAlloc.Authorization/RequestAuthorizationOptions.cs#L11)

The claims of the security context that hold the caller's scopes, each a space separated list. `scope` (OAuth 2.0) and `scp` (Microsoft Entra), under that name or the claim type ASP.NET Core maps it to, unless changed.

```csharp
public IList<string> ScopeClaims { get; }
```
