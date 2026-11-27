# Iso4217Currencies

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.457`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives.Registries`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.38910164e414562d3509d65b5a4e0133f1c911cc/src/Primitives/Registries/Iso4217Currencies.cs)

The alphabetic codes of ISO 4217 list one (current currency and funds codes), as published by SIX, the ISO 4217 maintenance agency, on 2026-09-17 (https://www.six-group.com/en/products-services/financial-information/data-standards.html). Update it from list-one.xml when the agency publishes an amendment.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives.Registries;

public static class Iso4217Currencies
{
    // ...
}
```

## Properties

- [Codes](/LaunchPad/api/rocket.surgery.launchpad.primitives/iso4217currencies/properties.md#codes) : `FrozenSet<string>` `static` `get` -- The codes.
