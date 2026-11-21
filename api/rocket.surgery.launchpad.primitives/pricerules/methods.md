# PriceRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [PriceRules](/LaunchPad/api/rocket.surgery.launchpad.primitives/pricerules.md)
- Kind: `Methods`
- Members: `1`

The rules of a price: an amount of money with no currency, such as a unit price.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<decimal>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.6c40644535fda9fb8f01149b7373b18e83589412/src/Primitives/StrongTypes/Price.cs#L14)

```csharp
public sealed class PriceRules
{
    public static StrongTypeDefinition<decimal> Define()
    {
        // ...
    }
}
```
