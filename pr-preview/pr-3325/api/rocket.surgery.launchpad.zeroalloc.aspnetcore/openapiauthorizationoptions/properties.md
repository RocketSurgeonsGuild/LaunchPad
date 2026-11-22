# OpenApiAuthorizationOptions Properties

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Type: [OpenApiAuthorizationOptions](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/openapiauthorizationoptions.md)
- Kind: `Properties`
- Members: `4`

How the scopes of an operation's policies are described in the OpenAPI document.

<a id="authorizationurl"></a>

## AuthorizationUrl

- Name: `AuthorizationUrl`
- Modifiers: `nullable` `get; set`
- Returns: `Uri?`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.AspNetCore/Authorization/OpenApiAuthorization.cs)

The authorization endpoint of the identity provider. With [OpenApiAuthorizationOptions.TokenUrl](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/openapiauthorizationoptions/properties.md#tokenurl), LaunchPad declares the scheme.

```csharp
public Uri? AuthorizationUrl { get; set; }
```

<a id="schemename"></a>

## SchemeName

- Name: `SchemeName`
- Modifiers: `get; set`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.AspNetCore/Authorization/OpenApiAuthorization.cs)

The name of the security scheme the operations require their scopes of. The application declares the scheme, or sets [OpenApiAuthorizationOptions.AuthorizationUrl](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/openapiauthorizationoptions/properties.md#authorizationurl) and [OpenApiAuthorizationOptions.TokenUrl](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/openapiauthorizationoptions/properties.md#tokenurl) for LaunchPad to declare it as OAuth 2.0; with neither, LaunchPad declares it as a bearer token.

```csharp
public string SchemeName { get; set; }
```

<a id="scopedescriptions"></a>

## ScopeDescriptions

- Name: `ScopeDescriptions`
- Modifiers: `get`
- Returns: `IDictionary<string, string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.AspNetCore/Authorization/OpenApiAuthorization.cs#L28)

What each scope grants, by scope, for the scheme LaunchPad declares. A scope without one is described by its own name.

```csharp
public IDictionary<string, string> ScopeDescriptions { get; }
```

<a id="tokenurl"></a>

## TokenUrl

- Name: `TokenUrl`
- Modifiers: `nullable` `get; set`
- Returns: `Uri?`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.AspNetCore/Authorization/OpenApiAuthorization.cs)

The token endpoint of the identity provider. With [OpenApiAuthorizationOptions.AuthorizationUrl](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/openapiauthorizationoptions/properties.md#authorizationurl), LaunchPad declares the scheme.

```csharp
public Uri? TokenUrl { get; set; }
```
