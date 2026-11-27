# PrimitivePatterns

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1294`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives.Advanced`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3354-merge.Sha.770f51e280ea81405e6ea681ce5c1987ac1bab2a/src/Primitives/obj/Release/net11.0/System.Text.RegularExpressions.Generator/System.Text.RegularExpressions.Generator.RegexGenerator/RegexGenerator.g.cs)

The patterns of the primitive kinds, which the code the generator writes for a kind matches the value against.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives.Advanced;

public static class PrimitivePatterns
{
    // ...
}
```

## Remarks

They are here, built by the regular expression generator when Primitives is built, because a `[GeneratedRegex]` the Primitives generator wrote into an application would never be implemented: one source generator doesn't see what another writes. Each has a one second timeout: a match that times out rejects the value.

## Properties

- [Email](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#email) : `Regex` `static` `get`
- [HexColor](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#hexcolor) : `Regex` `static` `get`
- [Iban](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#iban) : `Regex` `static` `get`
- [IPAddress](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#ipaddress) : `Regex` `static` `get`
- [Isbn](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#isbn) : `Regex` `static` `get`
- [LanguageCode](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#languagecode) : `Regex` `static` `get`
- [MacAddress](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#macaddress) : `Regex` `static` `get`
- [Phone](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#phone) : `Regex` `static` `get`
- [Slug](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#slug) : `Regex` `static` `get`
- [Username](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#username) : `Regex` `static` `get`
- [Vin](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#vin) : `Regex` `static` `get`
- [WhiteSpace](/LaunchPad/pr-preview/pr-3354/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#whitespace) : `Regex` `static` `get`
