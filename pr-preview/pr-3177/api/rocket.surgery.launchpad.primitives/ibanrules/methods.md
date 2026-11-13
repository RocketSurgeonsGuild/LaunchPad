# IbanRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives.md)
- Type: [IbanRules](/LaunchPad/pr-preview/pr-3177/api/rocket.surgery.launchpad.primitives/ibanrules.md)
- Kind: `Methods`
- Members: `1`

The rules of an IBAN, an international bank account number (ISO 13616-1), such as `GB82WEST12345698765432`.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`

```csharp
public sealed class IbanRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
