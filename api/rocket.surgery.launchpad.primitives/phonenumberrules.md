# PhoneNumberRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.423`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.bc6e38a474db052319ef3b044454738f3a433de2/src/Primitives/StrongTypes/PhoneNumber.cs)
- Implements: `IStrongTypeTemplate<string>`

The rules of a phone number: an ITU-T E.164 number in its international form, `+` followed by the country code and the subscriber number, such as `+14155552671`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class PhoneNumberRules
    : SuperStrong.Types.IStrongTypeTemplate<string>
{
    // ...
}
```

## Remarks

- Digits only after the `+`: no spaces, hyphens, brackets or extensions.
- The first digit (the start of the country code) is not 0, and there are 7 to 15 digits (E.164 §6.1 sets the maximum).
- The country code is not checked against the ITU assignments, nor the number against national numbering plans.

## Methods

- [Define](/LaunchPad/api/rocket.surgery.launchpad.primitives/phonenumberrules/methods.md#define) : `StrongTypeDefinition<string>` `static`
