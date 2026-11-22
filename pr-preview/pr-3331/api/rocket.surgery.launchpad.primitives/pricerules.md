# PriceRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1284`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3331-merge.Sha.4e9f7c28a9b18bc67da869f4b85f2484ef7df001/src/Primitives/StrongTypes/Price.cs)
- Implements: `IStrongTypeTemplate<decimal>`

The rules of a price: an amount of money with no currency, such as a unit price.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class PriceRules
    : SuperStrong.Types.IStrongTypeTemplate<decimal>
{
    // ...
}
```

## Remarks

Zero or more. A price is not rounded to a currency's minor unit; pair it with a [CurrencyCode](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.primitives/currencycode.md) or use [Money](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.primitives/money.md) when the currency matters.

## Methods

- [Define](/LaunchPad/pr-preview/pr-3331/api/rocket.surgery.launchpad.primitives/pricerules/methods.md#define) : `StrongTypeDefinition<decimal>` `static`
