# PriceRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3305/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1256`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3305-merge.Sha.f536a04ca4e28fbdc06da7c1e0f968bbeccd9e9a/src/Primitives/StrongTypes/Price.cs)
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

Zero or more. A price is not rounded to a currency's minor unit; pair it with a [CurrencyCode](/LaunchPad/pr-preview/pr-3305/api/rocket.surgery.launchpad.primitives/currencycode.md) or use [Money](/LaunchPad/pr-preview/pr-3305/api/rocket.surgery.launchpad.primitives/money.md) when the currency matters.

## Methods

- [Define](/LaunchPad/pr-preview/pr-3305/api/rocket.surgery.launchpad.primitives/pricerules/methods.md#define) : `StrongTypeDefinition<decimal>` `static`
