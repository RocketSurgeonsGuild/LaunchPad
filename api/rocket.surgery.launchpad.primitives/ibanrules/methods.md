# IbanRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [IbanRules](/LaunchPad/api/rocket.surgery.launchpad.primitives/ibanrules.md)
- Kind: `Methods`
- Members: `1`

The rules of an IBAN, an international bank account number (ISO 13616-1), such as `GB82WEST12345698765432`.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.5c5630f989568306f7aca9912bb0dcdaeccec9bd/src/Primitives/StrongTypes/Iban.cs#L26-L29)

```csharp
public sealed class IbanRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
