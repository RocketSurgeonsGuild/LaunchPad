# CurrencyCodeRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives.md)
- Type: [CurrencyCodeRules](/LaunchPad/pr-preview/pr-3193/api/rocket.surgery.launchpad.primitives/currencycoderules.md)
- Kind: `Methods`
- Members: `1`

The rules of a currency code: an ISO 4217 alphabetic code in current use, such as `USD` or `EUR`.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`

```csharp
public sealed class CurrencyCodeRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
