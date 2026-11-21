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
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.dfbe8776933590a8ca1efba66f9af5c4929d4975/src/Primitives/StrongTypes/Price.cs#L14)

```csharp
public sealed class PriceRules
{
    public static StrongTypeDefinition<decimal> Define()
    {
        // ...
    }
}
```
