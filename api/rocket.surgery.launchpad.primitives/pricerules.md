# PriceRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.447`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.afac89ac2f84551e9433e9fb435d06d6d3ff37f0/src/Primitives/StrongTypes/Price.cs)
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

Zero or more. A price is not rounded to a currency's minor unit; pair it with a [CurrencyCode](/LaunchPad/api/rocket.surgery.launchpad.primitives/currencycode.md) or use [Money](/LaunchPad/api/rocket.surgery.launchpad.primitives/money.md) when the currency matters.

## Methods

- [Define](/LaunchPad/api/rocket.surgery.launchpad.primitives/pricerules/methods.md#define) : `StrongTypeDefinition<decimal>` `static`
