# TaxRateRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1274`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3320-merge.Sha.2b65145aa6b8ea27d669d428ab7706034ee6420c/src/Primitives/StrongTypes/TaxRate.cs)
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

- [Define](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.primitives/taxraterules/methods.md#define) : `StrongTypeDefinition<decimal>` `static`
