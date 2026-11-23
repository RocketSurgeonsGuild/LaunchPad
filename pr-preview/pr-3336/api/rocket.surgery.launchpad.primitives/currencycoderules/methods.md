# CurrencyCodeRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives.md)
- Type: [CurrencyCodeRules](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/currencycoderules.md)
- Kind: `Methods`
- Members: `1`

The rules of a currency code: an ISO 4217 alphabetic code in current use, such as `USD` or `EUR`.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.11f685f6c8c4739a5c9c37bdc0dec7baf7e3894e/src/Primitives/StrongTypes/CurrencyCode.cs#L28-L31)

```csharp
public sealed class CurrencyCodeRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
