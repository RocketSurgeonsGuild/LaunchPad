# ScopeRequirement Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [ScopeRequirement](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/scoperequirement.md)
- Kind: `Constructors`
- Members: `1`

One scope attribute of a policy: scopes the caller must have all of, or any one of.

<a id="constructor"></a>
<a id="constructor-bool-string"></a>

## ScopeRequirement(bool, string[])

- Name: `Constructor(bool, string[])`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.1b696446ea1fb50dee983bcac42661335aad175b/src/ZeroAlloc.Authorization/ScopeRequirements.cs#L10-L16)

One scope attribute of a policy: scopes the caller must have all of, or any one of.

```csharp
public sealed class ScopeRequirement
{
    public ScopeRequirement(
        bool any,
        params string[] scopes)
    {
        // ...
    }
}
```

## Parameters

- `any` (`bool`)
  Whether any one of the scopes is enough.
- `scopes` (`string[]`)
  The scopes.
