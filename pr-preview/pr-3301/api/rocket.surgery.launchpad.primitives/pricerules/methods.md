# PriceRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.primitives.md)
- Type: [PriceRules](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.primitives/pricerules.md)
- Kind: `Methods`
- Members: `1`

The rules of a price: an amount of money with no currency, such as a unit price.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<decimal>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3301-merge.Sha.477a5d9575a97343218a11eba2a3d0088808ddab/src/Primitives/StrongTypes/Price.cs#L14)

```csharp
public sealed class PriceRules
{
    public static StrongTypeDefinition<decimal> Define()
    {
        // ...
    }
}
```
