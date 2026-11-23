# CurrencyCodeRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3335/api/rocket.surgery.launchpad.primitives.md)
- Type: [CurrencyCodeRules](/LaunchPad/pr-preview/pr-3335/api/rocket.surgery.launchpad.primitives/currencycoderules.md)
- Kind: `Methods`
- Members: `1`

The rules of a currency code: an ISO 4217 alphabetic code in current use, such as `USD` or `EUR`.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3335-merge.Sha.164159712cc333d1205a8448b12272b1815ca0c1/src/Primitives/StrongTypes/CurrencyCode.cs#L28-L31)

```csharp
public sealed class CurrencyCodeRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
