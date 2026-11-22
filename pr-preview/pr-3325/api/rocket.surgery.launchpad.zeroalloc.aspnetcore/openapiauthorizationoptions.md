# OpenApiAuthorizationOptions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Version: `2.0.1-pr.1298`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.1b696446ea1fb50dee983bcac42661335aad175b/src/ZeroAlloc.AspNetCore/Authorization/OpenApiAuthorization.cs)

How the scopes of an operation's policies are described in the OpenAPI document.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Authorization;

public sealed class OpenApiAuthorizationOptions
{
    // ...
}
```

## Constructors

- [OpenApiAuthorizationOptions](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/openapiauthorizationoptions/constructors.md#constructor)

## Properties

- [AuthorizationUrl](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/openapiauthorizationoptions/properties.md#authorizationurl) : `Uri?` `get; set` -- The authorization endpoint of the identity provider. With [OpenApiAuthorizationOptions.TokenUrl](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/openapiauthorizationoptions/properties.md#tokenurl), LaunchPad declares the scheme.
- [SchemeName](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/openapiauthorizationoptions/properties.md#schemename) : `string` `get; set` -- The name of the security scheme the operations require their scopes of. The application declares the scheme, or sets [OpenApiAuthorizationOptions.AuthorizationUrl](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/openapiauthorizationoptions/properties.md#authorizationurl) and [OpenApiAuthorizationOptions.TokenUrl](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/openapiauthorizationoptions/properties.md#tokenurl) for LaunchPad to declare it as OAuth 2.0; with neither, LaunchPad declares it as a bearer token.
- [ScopeDescriptions](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/openapiauthorizationoptions/properties.md#scopedescriptions) : `IDictionary<string, string>` `get` -- What each scope grants, by scope, for the scheme LaunchPad declares. A scope without one is described by its own name.
- [TokenUrl](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/openapiauthorizationoptions/properties.md#tokenurl) : `Uri?` `get; set` -- The token endpoint of the identity provider. With [OpenApiAuthorizationOptions.AuthorizationUrl](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/openapiauthorizationoptions/properties.md#authorizationurl), LaunchPad declares the scheme.
