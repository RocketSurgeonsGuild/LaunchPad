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
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.b95ef48c1c24b4d5eac70fa83d50ac9f4aff0224/src/ZeroAlloc.Authorization/RequestAuthorizationOptions.cs#L11)

The claims of the security context that hold the caller's scopes, each a space separated list. `scope` (OAuth 2.0) and `scp` (Microsoft Entra), under that name or the claim type ASP.NET Core maps it to, unless changed.

```csharp
public IList<string> ScopeClaims { get; }
```
