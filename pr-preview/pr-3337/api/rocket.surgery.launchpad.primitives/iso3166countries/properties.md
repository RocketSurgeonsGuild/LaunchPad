# Iso3166Countries Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives.md)
- Type: [Iso3166Countries](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/iso3166countries.md)
- Kind: `Properties`
- Members: `1`

The 249 officially assigned ISO 3166-1 alpha-2 codes (the list Debian iso-codes publishes from the ISO 3166 Maintenance Agency). User-assigned codes (such as XK), exceptionally reserved codes (such as UK and EU) and withdrawn codes are not in it. Update it when the agency publishes a newsletter that adds or withdraws a code.

<a id="codes"></a>

## Codes

- Name: `Codes`
- Modifiers: `static` `get`
- Returns: `FrozenSet<string>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3337-merge.Sha.7307fcdde362b2224f6c0071b6dfb1eaf21e5cff/src/Primitives/Registries/Iso3166Countries.cs#L16)

The codes.

```csharp
public static FrozenSet<string> Codes { get; }
```
