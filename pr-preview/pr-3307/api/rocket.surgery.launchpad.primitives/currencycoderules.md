# CurrencyCodeRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1258`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3307-merge.Sha.e1cd1b3273152fe1b84a7e7cf201a85e915d90b0/src/Primitives/StrongTypes/CurrencyCode.cs)
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

- [Define](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.primitives/currencycoderules/methods.md#define) : `StrongTypeDefinition<string>` `static`
