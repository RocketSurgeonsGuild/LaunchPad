# DateRange

- Kind: `record`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1187`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IEquatable<DateRange>`

A range of dates from [DateRange.Start](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/daterange/properties.md#start) to [DateRange.End](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/daterange/properties.md#end), both included. [DateRange.Start](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/daterange/properties.md#start) is never after [DateRange.End](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/daterange/properties.md#end); a range of one day has them equal.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed record DateRange
    : System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.DateRange>
{
    // ...
}
```

## Remarks

Like a strong type, it is valid by construction: [DateRange.From(DateOnly, DateOnly)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/daterange/methods.md#from-dateonly-dateonly) and the constructor throw for an invalid range, and [DateRange.TryFrom(DateOnly, DateOnly, DateRange?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/daterange/methods.md#tryfrom-dateonly-dateonly-daterange) returns `false`. It serializes as a JSON object with `start` and `end` members, and reading one checks it the same way.

## Constructors

- [DateRange(DateOnly, DateOnly)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/daterange/constructors.md#constructor-dateonly-dateonly) -- Creates a range from `start` to `end`.

## Properties

- [End](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/daterange/properties.md#end) : `DateOnly` `get` -- The last day of the range.
- [Start](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/daterange/properties.md#start) : `DateOnly` `get` -- The first day of the range.

## Methods

- [<Clone>$](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/daterange/methods.md#clone) : [DateRange](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/daterange.md)
- [Contains(DateOnly)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/daterange/methods.md#contains-dateonly) : `bool` -- Whether `date` is in the range.
- [Equals(object?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/daterange/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [Equals(DateRange?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/daterange/methods.md#equals-daterange) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [From(DateOnly, DateOnly)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/daterange/methods.md#from-dateonly-dateonly) : [DateRange](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/daterange.md) `static` -- Creates a range from `start` to `end`.
- [GetHashCode](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/daterange/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(DateRange?, DateRange?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/daterange/methods.md#op-equality-daterange-daterange) : `bool` `static`
- [op_Inequality(DateRange?, DateRange?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/daterange/methods.md#op-inequality-daterange-daterange) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/daterange/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [TryFrom(DateOnly, DateOnly, DateRange?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/daterange/methods.md#tryfrom-dateonly-dateonly-daterange) : `bool` `static` -- Creates a range from `start` to `end`, or returns `false` if it is invalid.
