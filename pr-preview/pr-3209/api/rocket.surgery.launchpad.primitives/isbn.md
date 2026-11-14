# Isbn

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3209/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1198`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<Isbn, string>`, `IComparable<Isbn>`, `IConvertible`, `IEquatable<Isbn>`, `IParsable<Isbn>`, `ISpanParsable<Isbn>`, `IEqualityOperators<Isbn, Isbn, bool>`

An ISBN-10 or ISBN-13, valid under [IsbnRules](/LaunchPad/pr-preview/pr-3209/api/rocket.surgery.launchpad.primitives/isbnrules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class Isbn
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.Isbn, string>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.Isbn>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.Isbn>,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.Isbn>,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Isbn>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.Isbn, Rocket.Surgery.LaunchPad.StrongTypes.Isbn, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/pr-preview/pr-3209/api/rocket.surgery.launchpad.primitives/isbn/methods.md#asprimitive) : `string`
- [CompareTo(Isbn?)](/LaunchPad/pr-preview/pr-3209/api/rocket.surgery.launchpad.primitives/isbn/methods.md#compareto-isbn) : `int`
- [Equals(Isbn?)](/LaunchPad/pr-preview/pr-3209/api/rocket.surgery.launchpad.primitives/isbn/methods.md#equals-isbn) : `bool`
- [Equals(object?)](/LaunchPad/pr-preview/pr-3209/api/rocket.surgery.launchpad.primitives/isbn/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(string)](/LaunchPad/pr-preview/pr-3209/api/rocket.surgery.launchpad.primitives/isbn/methods.md#from-string) : [Isbn](/LaunchPad/pr-preview/pr-3209/api/rocket.surgery.launchpad.primitives/isbn.md) `static`
- [GetHashCode](/LaunchPad/pr-preview/pr-3209/api/rocket.surgery.launchpad.primitives/isbn/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Isbn?, Isbn?)](/LaunchPad/pr-preview/pr-3209/api/rocket.surgery.launchpad.primitives/isbn/methods.md#op-equality-isbn-isbn) : `bool` `static`
- [op_Inequality(Isbn?, Isbn?)](/LaunchPad/pr-preview/pr-3209/api/rocket.surgery.launchpad.primitives/isbn/methods.md#op-inequality-isbn-isbn) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3209/api/rocket.surgery.launchpad.primitives/isbn/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [TryFrom(string, Isbn)](/LaunchPad/pr-preview/pr-3209/api/rocket.surgery.launchpad.primitives/isbn/methods.md#tryfrom-string-isbn) : `bool` `static`
