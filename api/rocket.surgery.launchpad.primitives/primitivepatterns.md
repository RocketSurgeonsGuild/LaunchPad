# PrimitivePatterns

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.468`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives.Advanced`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.b4901080d5186942ba234e171537f676572c6f35/src/Primitives/obj/Release/net11.0/System.Text.RegularExpressions.Generator/System.Text.RegularExpressions.Generator.RegexGenerator/RegexGenerator.g.cs)

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

- [Email](/LaunchPad/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#email) : `Regex` `static` `get`
- [HexColor](/LaunchPad/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#hexcolor) : `Regex` `static` `get`
- [Iban](/LaunchPad/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#iban) : `Regex` `static` `get`
- [IPAddress](/LaunchPad/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#ipaddress) : `Regex` `static` `get`
- [Isbn](/LaunchPad/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#isbn) : `Regex` `static` `get`
- [LanguageCode](/LaunchPad/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#languagecode) : `Regex` `static` `get`
- [MacAddress](/LaunchPad/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#macaddress) : `Regex` `static` `get`
- [Phone](/LaunchPad/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#phone) : `Regex` `static` `get`
- [Slug](/LaunchPad/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#slug) : `Regex` `static` `get`
- [Username](/LaunchPad/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#username) : `Regex` `static` `get`
- [Vin](/LaunchPad/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#vin) : `Regex` `static` `get`
- [WhiteSpace](/LaunchPad/api/rocket.surgery.launchpad.primitives/primitivepatterns/properties.md#whitespace) : `Regex` `static` `get`
