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
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/ScopeRequirements.cs#L10-L16)

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
