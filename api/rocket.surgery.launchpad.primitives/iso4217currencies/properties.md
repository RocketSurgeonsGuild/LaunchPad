# Iso4217Currencies Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [Iso4217Currencies](/LaunchPad/api/rocket.surgery.launchpad.primitives/iso4217currencies.md)
- Kind: `Properties`
- Members: `1`

The alphabetic codes of ISO 4217 list one (current currency and funds codes), as published by SIX, the ISO 4217 maintenance agency, on 2026-09-17 (https://www.six-group.com/en/products-services/financial-information/data-standards.html). Update it from list-one.xml when the agency publishes an amendment.

<a id="codes"></a>

## Codes

- Name: `Codes`
- Modifiers: `static` `get`
- Returns: `FrozenSet<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.0c205f7c0f7c99d017c3755b484180de4bcb850a/src/Primitives/Registries/Iso4217Currencies.cs#L16)

The codes.

```csharp
public static FrozenSet<string> Codes { get; }
```
