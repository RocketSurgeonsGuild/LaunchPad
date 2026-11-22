# PhoneNumberRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1284`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.108cb13ce492322587639698c87355d3b8fcee1b/src/Primitives/StrongTypes/PhoneNumber.cs)
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

- [Define](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/phonenumberrules/methods.md#define) : `StrongTypeDefinition<string>` `static`
