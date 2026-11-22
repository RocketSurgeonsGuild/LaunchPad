# PriceRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives.md)
- Type: [PriceRules](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/pricerules.md)
- Kind: `Methods`
- Members: `1`

The rules of a price: an amount of money with no currency, such as a unit price.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<decimal>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.fb40f4ae51769f679795c3f90ed1c13a62505cf5/src/Primitives/StrongTypes/Price.cs#L14)

```csharp
public sealed class PriceRules
{
    public static StrongTypeDefinition<decimal> Define()
    {
        // ...
    }
}
```
