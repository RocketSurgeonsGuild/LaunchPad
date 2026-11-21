# EmailAddressRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [EmailAddressRules](/LaunchPad/api/rocket.surgery.launchpad.primitives/emailaddressrules.md)
- Kind: `Methods`
- Members: `1`

The rules of an email address: an RFC 5322 `addr-spec` in the form RFC 5321 delivers mail to.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.d5743b8445e2048c34f1a4b585f3eb4d862008a8/src/Primitives/StrongTypes/EmailAddress.cs#L45-L53)

```csharp
public sealed class EmailAddressRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
