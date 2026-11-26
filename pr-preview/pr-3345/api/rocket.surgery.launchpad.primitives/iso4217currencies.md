# Iso4217Currencies

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1290`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives.Registries`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3345-merge.Sha.a9c0325e0c8c70bc9d0569ac4df1fe5d29ffa4ec/src/Primitives/Registries/Iso4217Currencies.cs)

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

- [Codes](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.primitives/iso4217currencies/properties.md#codes) : `FrozenSet<string>` `static` `get` -- The codes.
