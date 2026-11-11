# HexColor

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3191/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1178`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<HexColor, string>`, `IComparable<HexColor>`, `IConvertible`, `IEquatable<HexColor>`, `IParsable<HexColor>`, `ISpanParsable<HexColor>`, `IEqualityOperators<HexColor, HexColor, bool>`

A hexadecimal color, valid under [HexColorRules](/LaunchPad/pr-preview/pr-3191/api/rocket.surgery.launchpad.primitives/hexcolorrules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class HexColor
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.HexColor, string>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.HexColor>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.HexColor>,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.HexColor>,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.HexColor>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.HexColor, Rocket.Surgery.LaunchPad.StrongTypes.HexColor, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/pr-preview/pr-3191/api/rocket.surgery.launchpad.primitives/hexcolor/methods.md#asprimitive) : `string`
- [CompareTo(HexColor?)](/LaunchPad/pr-preview/pr-3191/api/rocket.surgery.launchpad.primitives/hexcolor/methods.md#compareto-hexcolor) : `int`
- [Equals(HexColor?)](/LaunchPad/pr-preview/pr-3191/api/rocket.surgery.launchpad.primitives/hexcolor/methods.md#equals-hexcolor) : `bool`
- [Equals(object?)](/LaunchPad/pr-preview/pr-3191/api/rocket.surgery.launchpad.primitives/hexcolor/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(string)](/LaunchPad/pr-preview/pr-3191/api/rocket.surgery.launchpad.primitives/hexcolor/methods.md#from-string) : [HexColor](/LaunchPad/pr-preview/pr-3191/api/rocket.surgery.launchpad.primitives/hexcolor.md) `static`
- [GetHashCode](/LaunchPad/pr-preview/pr-3191/api/rocket.surgery.launchpad.primitives/hexcolor/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(HexColor?, HexColor?)](/LaunchPad/pr-preview/pr-3191/api/rocket.surgery.launchpad.primitives/hexcolor/methods.md#op-equality-hexcolor-hexcolor) : `bool` `static`
- [op_Inequality(HexColor?, HexColor?)](/LaunchPad/pr-preview/pr-3191/api/rocket.surgery.launchpad.primitives/hexcolor/methods.md#op-inequality-hexcolor-hexcolor) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3191/api/rocket.surgery.launchpad.primitives/hexcolor/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [TryFrom(string, HexColor)](/LaunchPad/pr-preview/pr-3191/api/rocket.surgery.launchpad.primitives/hexcolor/methods.md#tryfrom-string-hexcolor) : `bool` `static`
