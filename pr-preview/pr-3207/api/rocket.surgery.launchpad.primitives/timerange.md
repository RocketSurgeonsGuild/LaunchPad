# TimeRange

- Kind: `record`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1218`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`
- Implements: `IEquatable<TimeRange>`

A range of times of day from [TimeRange.Start](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/timerange/properties.md#start) to [TimeRange.End](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/timerange/properties.md#end) within one day. [TimeRange.Start](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/timerange/properties.md#start) is never after [TimeRange.End](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/timerange/properties.md#end), so a range cannot run past midnight; an empty range has them equal.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public sealed record TimeRange
    : System.IEquatable<Rocket.Surgery.LaunchPad.StrongTypes.TimeRange>
{
    // ...
}
```

## Remarks

Like a strong type, it is valid by construction: [TimeRange.From(TimeOnly, TimeOnly)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/timerange/methods.md#from-timeonly-timeonly) and the constructor throw for an invalid range, and [TimeRange.TryFrom(TimeOnly, TimeOnly, TimeRange?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/timerange/methods.md#tryfrom-timeonly-timeonly-timerange) returns `false`. It serializes as a JSON object with `start` and `end` members, and reading one checks it the same way.

## Constructors

- [TimeRange(TimeOnly, TimeOnly)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/timerange/constructors.md#constructor-timeonly-timeonly) -- Creates a range from `start` to `end`.

## Properties

- [Duration](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/timerange/properties.md#duration) : `TimeSpan` `get` -- How long the range is.
- [End](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/timerange/properties.md#end) : `TimeOnly` `get` -- The time the range ends at.
- [Start](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/timerange/properties.md#start) : `TimeOnly` `get` -- The time the range starts at.

## Methods

- [<Clone>$](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/timerange/methods.md#clone) : [TimeRange](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/timerange.md)
- [Equals(object?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/timerange/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [Equals(TimeRange?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/timerange/methods.md#equals-timerange) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [From(TimeOnly, TimeOnly)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/timerange/methods.md#from-timeonly-timeonly) : [TimeRange](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/timerange.md) `static` -- Creates a range from `start` to `end`.
- [GetHashCode](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/timerange/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(TimeRange?, TimeRange?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/timerange/methods.md#op-equality-timerange-timerange) : `bool` `static`
- [op_Inequality(TimeRange?, TimeRange?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/timerange/methods.md#op-inequality-timerange-timerange) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/timerange/methods.md#tostring) : `string` -- Returns a string that represents the current object.
- [TryFrom(TimeOnly, TimeOnly, TimeRange?)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.primitives/timerange/methods.md#tryfrom-timeonly-timeonly-timerange) : `bool` `static` -- Creates a range from `start` to `end`, or returns `false` if it is invalid.
