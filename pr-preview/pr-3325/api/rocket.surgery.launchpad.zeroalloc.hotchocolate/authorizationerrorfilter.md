# AuthorizationErrorFilter

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Version: `2.0.1-pr.1291`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate.Authorization`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.HotChocolate/Authorization/AuthorizationErrorFilter.cs)
- Implements: `IErrorFilter`

Reports a field HotChocolate's authorization rejected as the `Unauthorized` or `Forbidden` case of a union response is reported: `UNAUTHENTICATED` or `FORBIDDEN` as the code, and the case name in `extensions.case`. A client reads one shape, whether the edge or the mediator pipeline rejected the call.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.HotChocolate.Authorization;

public sealed class AuthorizationErrorFilter
    : HotChocolate.Execution.IErrorFilter
{
    // ...
}
```

## Constructors

- [AuthorizationErrorFilter](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/authorizationerrorfilter/constructors.md#constructor)

## Methods

- [OnError(IError)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/authorizationerrorfilter/methods.md#onerror-ierror) : `IError` -- OnError is called whenever an error occurred during execution of a query.

## Fields

- [NotAuthenticated](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/authorizationerrorfilter/fields.md#notauthenticated) : `string` `static` -- HotChocolate's code for a caller that is not authenticated.
- [NotAuthorized](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/authorizationerrorfilter/fields.md#notauthorized) : `string` `static` -- HotChocolate's code for a caller that is not allowed.
