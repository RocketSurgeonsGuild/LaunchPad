# RequiresScopesAttribute Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [RequiresScopesAttribute](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/requiresscopesattribute.md)
- Kind: `Constructors`
- Members: `1`

On a `[Policy]` class: the scopes the policy requires of the caller, all of them. LaunchPad checks them before the policy runs, and publishes them as the scopes of every operation whose request names the policy.

<a id="constructor"></a>
<a id="constructor-string"></a>

## RequiresScopesAttribute(string[])

- Name: `Constructor(string[])`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.485b8b823e39f66de1cc144d4dd7b3bfd6b45f6e/src/ZeroAlloc.Authorization/RequiresScopesAttribute.cs#L10-L13)

On a `[Policy]` class: the scopes the policy requires of the caller, all of them. LaunchPad checks them before the policy runs, and publishes them as the scopes of every operation whose request names the policy.

```csharp
public sealed class RequiresScopesAttribute
{
    public RequiresScopesAttribute(
        params string[] scopes)
    {
        // ...
    }
}
```

## Parameters

- `scopes` (`string[]`)
  The scopes, as they appear in the caller's token.
