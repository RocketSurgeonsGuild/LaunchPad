# TaxRateRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.434`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.dfbe8776933590a8ca1efba66f9af5c4929d4975/src/Primitives/StrongTypes/TaxRate.cs)
- Implements: `IStrongTypeTemplate<decimal>`

The rules of a tax rate, as a percentage of the taxed amount.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class TaxRateRules
    : SuperStrong.Types.IStrongTypeTemplate<decimal>
{
    // ...
}
```

## Remarks

0 to 100 inclusive, as a number of hundredths: 20 is 20 %. No tax uses more than 100 %, and none is negative.

## Methods

- [Define](/LaunchPad/api/rocket.surgery.launchpad.primitives/taxraterules/methods.md#define) : `StrongTypeDefinition<decimal>` `static`
