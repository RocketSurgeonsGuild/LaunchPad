# AuthorizationErrorFilter Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Type: [AuthorizationErrorFilter](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/authorizationerrorfilter.md)
- Kind: `Constructors`
- Members: `1`

Reports a field HotChocolate's authorization rejected as the `Unauthorized` or `Forbidden` case of a union response is reported: `UNAUTHENTICATED` or `FORBIDDEN` as the code, and the case name in `extensions.case`. A client reads one shape, whether the edge or the mediator pipeline rejected the call.

<a id="constructor"></a>

## AuthorizationErrorFilter

- Name: `Constructor`

```csharp
public sealed class AuthorizationErrorFilter
{
    public AuthorizationErrorFilter()
    {
        // ...
    }
}
```
