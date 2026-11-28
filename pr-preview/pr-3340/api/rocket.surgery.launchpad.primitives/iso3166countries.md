# Iso3166Countries

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3340/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1300`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives.Registries`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3340-merge.Sha.1fd0124e0c52f2b8f0975bb6854b707a6a798292/src/Primitives/Registries/Iso3166Countries.cs)

The 249 officially assigned ISO 3166-1 alpha-2 codes (the list Debian iso-codes publishes from the ISO 3166 Maintenance Agency). User-assigned codes (such as XK), exceptionally reserved codes (such as UK and EU) and withdrawn codes are not in it. Update it when the agency publishes a newsletter that adds or withdraws a code.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives.Registries;

public static class Iso3166Countries
{
    // ...
}
```

## Properties

- [Codes](/LaunchPad/pr-preview/pr-3340/api/rocket.surgery.launchpad.primitives/iso3166countries/properties.md#codes) : `FrozenSet<string>` `static` `get` -- The codes.
