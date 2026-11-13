# IsbnRules

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1183`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IStrongTypeTemplate<string>`

The rules of an ISBN, an international standard book number (ISO 2108): an ISBN-13 such as `9780306406157` or an ISBN-10 such as `0306406152`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed class IsbnRules
    : SuperStrong.Types.IStrongTypeTemplate<string>
{
    // ...
}
```

## Remarks

- The compact form: digits only, without the hyphens or spaces of the printed form, and no `ISBN` prefix. An ISBN-10 may end in an upper-case `X` (check value 10); lower case `x` is rejected.
- An ISBN-13 starts with the 978 or 979 prefix of the book industry.
- The check digit must be right: weights 10 to 1 modulo 11 for an ISBN-10, weights 1 and 3 modulo 10 for an ISBN-13. The registration group and registrant are not checked against the ISBN ranges.
- ISBN-10 and ISBN-13 values are kept as given; the same book in both forms is two different values.

## Methods

- [Define](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.primitives/isbnrules/methods.md#define) : `StrongTypeDefinition<string>` `static`
