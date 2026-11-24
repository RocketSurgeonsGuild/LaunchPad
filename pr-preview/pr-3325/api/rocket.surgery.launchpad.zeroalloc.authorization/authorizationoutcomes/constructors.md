# AuthorizationOutcomes Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.Authorization](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization.md)
- Type: [AuthorizationOutcomes](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.authorization/authorizationoutcomes.md)
- Kind: `Constructors`
- Members: `1`

What each policy decided for the current call, by policy name and arguments. The edge and the mediator pipeline share it through the call's service scope, so a policy runs once.

<a id="constructor"></a>

## AuthorizationOutcomes

- Name: `Constructor`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/ZeroAlloc.Authorization/AuthorizationOutcomes.cs#L12)

```csharp
public sealed class AuthorizationOutcomes
{
    public AuthorizationOutcomes()
    {
        // ...
    }
}
```
