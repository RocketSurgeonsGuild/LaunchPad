# AuthorizationBehavior Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [AuthorizationBehavior](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationbehavior.md)
- Kind: `Constructors`
- Members: `1`

The ZeroAlloc.Mediator pipeline behavior that checks a request's policies before anything else runs. A caller with no identity answers the `Foundation.Unauthorized` case of the response union, and a caller a policy denies answers `Foundation.Forbidden`; a request without policies passes straight through.

<a id="constructor"></a>

## AuthorizationBehavior

- Name: `Constructor`

```csharp
public sealed class AuthorizationBehavior
{
    public AuthorizationBehavior()
    {
        // ...
    }
}
```
