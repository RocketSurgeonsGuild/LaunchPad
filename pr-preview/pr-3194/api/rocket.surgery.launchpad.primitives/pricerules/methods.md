# PriceRules Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives.md)
- Type: [PriceRules](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/pricerules.md)
- Kind: `Methods`
- Members: `1`

The rules of a price: an amount of money with no currency, such as a unit price.

<a id="define"></a>

## Define

- Name: `Define`
- Modifiers: `static`
- Returns: `StrongTypeDefinition<decimal>`

```csharp
public sealed class PriceRules
{
    public static StrongTypeDefinition<decimal> Define()
    {
        // ...
    }
}
```
