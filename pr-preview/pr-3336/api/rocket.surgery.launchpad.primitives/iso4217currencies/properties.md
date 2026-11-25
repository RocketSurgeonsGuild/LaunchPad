# Iso4217Currencies Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives.md)
- Type: [Iso4217Currencies](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/iso4217currencies.md)
- Kind: `Properties`
- Members: `1`

The alphabetic codes of ISO 4217 list one (current currency and funds codes), as published by SIX, the ISO 4217 maintenance agency, on 2026-09-17 (https://www.six-group.com/en/products-services/financial-information/data-standards.html). Update it from list-one.xml when the agency publishes an amendment.

<a id="codes"></a>

## Codes

- Name: `Codes`
- Modifiers: `static` `get`
- Returns: `FrozenSet<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.7cda342c7a30094b955266549c4928a5c52b02bb/src/Primitives/Registries/Iso4217Currencies.cs#L16)

The codes.

```csharp
public static FrozenSet<string> Codes { get; }
```
