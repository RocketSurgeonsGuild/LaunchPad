# AuthorizationErrorFilter Fields

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Type: [AuthorizationErrorFilter](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/authorizationerrorfilter.md)
- Kind: `Fields`
- Members: `2`

Reports a field HotChocolate's authorization rejected as the `Unauthorized` or `Forbidden` case of a union response is reported: `UNAUTHENTICATED` or `FORBIDDEN` as the code, and the case name in `extensions.case`. A client reads one shape, whether the edge or the mediator pipeline rejected the call.

<a id="notauthenticated"></a>

## NotAuthenticated

- Name: `NotAuthenticated`
- Modifiers: `static` `const`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.cda885391825d90598517a0aef31b3494df7c9f7/src/ZeroAlloc.HotChocolate/Authorization/AuthorizationErrorFilter.cs)

HotChocolate's code for a caller that is not authenticated.

```csharp
public const static string NotAuthenticated
```

<a id="notauthorized"></a>

## NotAuthorized

- Name: `NotAuthorized`
- Modifiers: `static` `const`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.cda885391825d90598517a0aef31b3494df7c9f7/src/ZeroAlloc.HotChocolate/Authorization/AuthorizationErrorFilter.cs)

HotChocolate's code for a caller that is not allowed.

```csharp
public const static string NotAuthorized
```
