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
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.bc6e38a474db052319ef3b044454738f3a433de2/src/Primitives/StrongTypes/Iban.cs#L26-L29)

```csharp
public sealed class IbanRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
