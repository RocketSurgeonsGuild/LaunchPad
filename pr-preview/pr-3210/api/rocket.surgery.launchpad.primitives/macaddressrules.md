# MacAddressRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1190`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongTypeTemplate<string>`

The rules of a MAC address, an IEEE EUI-48, in the IEEE 802 hexadecimal representation: six groups of two upper-case hexadecimal digits separated by hyphens, such as `00-1A-2B-3C-4D-5E`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class MacAddressRules
    : SuperStrong.Types.IStrongTypeTemplate<string>
{
    // ...
}
```

## Remarks

- IEEE 802 §8.1 writes each octet as two upper-case hexadecimal digits separated by hyphens. Colons ( `00:1a:2b:3c:4d:5e`), dots, lower case and the bare 12 digits are rejected rather than converted, since SuperStrong validates without changing the value.
- EUI-64 addresses are rejected. Any EUI-48 is accepted, including broadcast and multicast addresses.

## Methods

- [Define](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/macaddressrules/methods.md#define) : `StrongTypeDefinition<string>` `static`
