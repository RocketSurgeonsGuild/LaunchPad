# CurrencyCodeRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1188`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongTypeTemplate<string>`

The rules of a currency code: an ISO 4217 alphabetic code in current use, such as `USD` or `EUR`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class CurrencyCodeRules
    : SuperStrong.Types.IStrongTypeTemplate<string>
{
    // ...
}
```

## Remarks

- The code must be on ISO 4217 list one (current currencies and funds), a list kept in this package as published on 2026-09-17. It includes the funds codes ( `USN`, `CLF`), precious metals ( `XAU`), `XTS` (testing) and `XXX` (no currency); withdrawn codes ( `HRK`, `BGN`, `ANG`) are rejected.
- ISO 4217 writes codes in upper case. SuperStrong validates without changing the value, so lower case ( `usd`) is rejected rather than converted.

## Methods

- [Define](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/currencycoderules/methods.md#define) : `StrongTypeDefinition<string>` `static`
