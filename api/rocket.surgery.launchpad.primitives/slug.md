# Slug

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.340`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongType<Slug, string>`, `IComparable<Slug>`, `IConvertible`, `IEquatable<Slug>`, `IParsable<Slug>`, `ISpanParsable<Slug>`, `IEqualityOperators<Slug, Slug, bool>`

A URL slug, valid under [SlugRules](/LaunchPad/api/rocket.surgery.launchpad.primitives/slugrules.md).

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class Slug
    : SuperStrong.Types.IStrongType<Rocket.Surgery.LaunchPad.StrongTypes.Slug, string>,
      System.IComparable<Rocket.Surgery.LaunchPad.StrongTypes.Slug>,
      System.IConvertible,
      System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.Slug>,
      System.IParsable<Rocket.Surgery.LaunchPad.StrongTypes.Slug>,
      System.ISpanParsable<Rocket.Surgery.LaunchPad.StrongTypes.Slug>,
      System.Numerics.IEqualityOperators<Rocket.Surgery.LaunchPad.StrongTypes.Slug, Rocket.Surgery.LaunchPad.StrongTypes.Slug, bool>
{
    // ...
}
```

## Methods

- [AsPrimitive](/LaunchPad/api/rocket.surgery.launchpad.primitives/slug/methods.md#asprimitive) : `string`
- [CompareTo(Slug?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/slug/methods.md#compareto-slug) : `int`
- [Equals(Slug?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/slug/methods.md#equals-slug) : `bool`
- [Equals(object?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/slug/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [From(string)](/LaunchPad/api/rocket.surgery.launchpad.primitives/slug/methods.md#from-string) : [Slug](/LaunchPad/api/rocket.surgery.launchpad.primitives/slug.md) `static`
- [GetHashCode](/LaunchPad/api/rocket.surgery.launchpad.primitives/slug/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(Slug?, Slug?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/slug/methods.md#op-equality-slug-slug) : `bool` `static`
- [op_Inequality(Slug?, Slug?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/slug/methods.md#op-inequality-slug-slug) : `bool` `static`
- [ToString](/LaunchPad/api/rocket.surgery.launchpad.primitives/slug/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [TryFrom(string, Slug)](/LaunchPad/api/rocket.surgery.launchpad.primitives/slug/methods.md#tryfrom-string-slug) : `bool` `static`
