# IbanRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.primitives.md)
- Type: [IbanRules](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.primitives/ibanrules.md)
- Kind: `Methods`
- Members: `1`

The rules of an IBAN, an international bank account number (ISO 13616-1), such as `GB82WEST12345698765432`.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3320-merge.Sha.bb93b2eb5e069135a2caef17855a85e8a18d22af/src/Primitives/StrongTypes/Iban.cs#L26-L29)

```csharp
public sealed class IbanRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
