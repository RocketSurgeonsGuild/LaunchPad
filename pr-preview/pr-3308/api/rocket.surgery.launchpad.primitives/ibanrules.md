# IbanRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1261`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3308-merge.Sha.b5321c7138bef72baa9f203b3f824fa7207c9dcf/src/Primitives/StrongTypes/Iban.cs)
- Implements: `IStrongTypeTemplate<string>`

The rules of an IBAN, an international bank account number (ISO 13616-1), such as `GB82WEST12345698765432`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class IbanRules
    : SuperStrong.Types.IStrongTypeTemplate<string>
{
    // ...
}
```

## Remarks

- The electronic format: no spaces, two upper-case letters for the country, two check digits, then 11 to 30 upper-case letters and digits, 15 to 34 characters in all. The printed format with spaces, and lower case, are rejected.
- The check digits are 02 to 98, and the number passes the ISO 7064 MOD 97-10 check.
- The country's IBAN length and BBAN format (the SWIFT IBAN registry) are not checked, and the country letters are not checked against ISO 3166-1 (the registry includes codes such as `XK`).

## Methods

- [Define](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.primitives/ibanrules/methods.md#define) : `StrongTypeDefinition<string>` `static`
