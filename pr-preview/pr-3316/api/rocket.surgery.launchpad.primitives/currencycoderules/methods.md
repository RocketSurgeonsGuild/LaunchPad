# CurrencyCodeRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3316/api/rocket.surgery.launchpad.primitives.md)
- Type: [CurrencyCodeRules](/LaunchPad/pr-preview/pr-3316/api/rocket.surgery.launchpad.primitives/currencycoderules.md)
- Kind: `Methods`
- Members: `1`

The rules of a currency code: an ISO 4217 alphabetic code in current use, such as `USD` or `EUR`.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3316-merge.Sha.5b7e33a497070ffcf95e9f2c28132b3e4913cfd6/src/Primitives/StrongTypes/CurrencyCode.cs#L28-L31)

```csharp
public sealed class CurrencyCodeRules
{
    public static StrongTypeDefinition<string> Define()
    {
        // ...
    }
}
```
