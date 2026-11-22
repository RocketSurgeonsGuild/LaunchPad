# RequireScopeAttribute Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequireScopeAttribute](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requirescopeattribute.md)
- Kind: `Constructors`
- Members: `1`

On a `[Policy]` class: scopes the policy requires of the caller, all of them. The policy is then what its scope attributes say and nothing else: LaunchPad writes its `EvaluateAsync`, which checks the caller's scopes, and states the scopes as those of every operation whose request names the policy.

<a id="constructor"></a>
<a id="constructor-string"></a>

## RequireScopeAttribute(string[])

- Name: `Constructor(string[])`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.1b696446ea1fb50dee983bcac42661335aad175b/src/ZeroAlloc.Authorization/ScopeAttributes.cs#L15-L18)

On a `[Policy]` class: scopes the policy requires of the caller, all of them. The policy is then what its scope attributes say and nothing else: LaunchPad writes its `EvaluateAsync`, which checks the caller's scopes, and states the scopes as those of every operation whose request names the policy.

```csharp
public sealed class RequireScopeAttribute
{
    public RequireScopeAttribute(
        params string[] scopes)
    {
        // ...
    }
}
```

## Parameters

- `scopes` (`string[]`)
  The scopes, as they appear in the caller's `scope` claim.

## Remarks

The class is partial and lists `IAuthorizationPolicy`. Several attributes on a class must all be satisfied, alongside its [RequireAnyScopeAttribute](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requireanyscopeattribute.md) s.
