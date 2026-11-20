# CountryCodeRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3306/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1258`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3306-merge.Sha.7731056fc28c1fb31fe802623d663df67f58b451/src/Primitives/StrongTypes/CountryCode.cs)
- Implements: `IStrongTypeTemplate<string>`

The rules of a country code: an officially assigned ISO 3166-1 alpha-2 code, such as `US` or `DE`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class CountryCodeRules
    : SuperStrong.Types.IStrongTypeTemplate<string>
{
    // ...
}
```

## Remarks

- The code must be one of the 249 officially assigned codes, a list kept in this package. User-assigned codes ( `XK` for Kosovo, `ZZ`), exceptionally reserved codes ( `UK`, `EU`) and withdrawn codes ( `AN`, `CS`) are rejected.
- ISO 3166-1 writes codes in upper case. SuperStrong validates without changing the value, so lower case ( `us`) is rejected rather than converted.
- The list is not read from `Globalization.RegionInfo`: it accepts user-assigned and withdrawn codes, misses some assigned ones, and depends on the platform's ICU data and on invariant globalization mode.

## Methods

- [Define](/LaunchPad/pr-preview/pr-3306/api/rocket.surgery.launchpad.primitives/countrycoderules/methods.md#define) : `StrongTypeDefinition<string>` `static`
