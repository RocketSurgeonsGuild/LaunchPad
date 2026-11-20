# EmailAddressRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3306/api/rocket.surgery.launchpad.primitives.md)
- Type: [EmailAddressRules](/LaunchPad/pr-preview/pr-3306/api/rocket.surgery.launchpad.primitives/emailaddressrules.md)
- Kind: `Methods`
- Members: `1`

The rules of an email address: an RFC 5322 `addr-spec` in the form RFC 5321 delivers mail to.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3306-merge.Sha.7731056fc28c1fb31fe802623d663df67f58b451/src/Primitives/StrongTypes/EmailAddress.cs#L45-L53)

```csharp
public sealed class EmailAddressRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
