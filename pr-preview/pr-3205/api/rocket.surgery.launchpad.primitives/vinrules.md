# VinRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1192`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongTypeTemplate<string>`

The rules of a VIN, a vehicle identification number (ISO 3779), such as `1M8GDM9AXKP042788`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class VinRules
    : SuperStrong.Types.IStrongTypeTemplate<string>
{
    // ...
}
```

## Remarks

- 17 characters: digits and upper-case letters except `I`, `O` and `Q` (ISO 3779 §5.4). Lower case is rejected.
- The ninth character is the check digit ( `0` - `9` or `X`) computed as North America requires (49 CFR 565.15). ISO 3779 does not require a check digit, so VINs of vehicles made for markets that do not use one can be rejected; an application that has them declares its own type with rules that leave the check out.
- The manufacturer identifier is not checked against the SAE registry, nor the model year character.

## Methods

- [Define](/LaunchPad/pr-preview/pr-3205/api/rocket.surgery.launchpad.primitives/vinrules/methods.md#define) : `StrongTypeDefinition<string>` `static`
