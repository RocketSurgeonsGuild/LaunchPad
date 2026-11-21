# PolicyOutcome Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [PolicyOutcome](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/policyoutcome.md)
- Kind: `Constructors`
- Members: `1`

What one policy decided: allowed, or denied with the scopes the caller lacks or the policy's own failure.

<a id="constructor"></a>
<a id="constructor-bool-ireadonlylist-string-authorizationfailure"></a>

## PolicyOutcome(bool, IReadOnlyList<string>, AuthorizationFailure?)

- Name: `Constructor(bool, IReadOnlyList<string>, AuthorizationFailure?)`

What one policy decided: allowed, or denied with the scopes the caller lacks or the policy's own failure.

```csharp
public struct PolicyOutcome
{
    public PolicyOutcome(
        bool Allowed,
        IReadOnlyList<string> MissingScopes,
        AuthorizationFailure? Failure)
    {
        // ...
    }
}
```

## Parameters

- `Allowed` (`bool`)
  The policy allows the caller.
- `MissingScopes` (`IReadOnlyList<string>`)
  The declared scopes the caller lacks; empty when the policy's own logic decided.
- `Failure` (`AuthorizationFailure?`)
  The policy's own failure, when its logic denied the caller.
