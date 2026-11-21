# AuthorizationDecision Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [AuthorizationDecision](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationdecision.md)
- Kind: `Constructors`
- Members: `1`

What a request's policies decide for a caller.

<a id="constructor"></a>
<a id="constructor-problem-ireadonlylist-string"></a>

## AuthorizationDecision(Problem?, IReadOnlyList<string>)

- Name: `Constructor(Problem?, IReadOnlyList<string>)`

What a request's policies decide for a caller.

```csharp
public struct AuthorizationDecision
{
    public AuthorizationDecision(
        Problem? Problem,
        IReadOnlyList<string> RequiredScopes)
    {
        // ...
    }
}
```

## Parameters

- `Problem` (`Problem?`)
  Null when allowed; otherwise `Foundation.Unauthorized` or `Foundation.Forbidden`.
- `RequiredScopes` (`IReadOnlyList<string>`)
  The scopes of the requirement that denied the caller, for the challenge; empty otherwise.
