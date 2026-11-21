# TimeRange Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.primitives.md)
- Type: [TimeRange](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.primitives/timerange.md)
- Kind: `Constructors`
- Members: `1`

A range of times of day from [TimeRange.Start](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.primitives/timerange/properties.md#start) to [TimeRange.End](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.primitives/timerange/properties.md#end) within one day. [TimeRange.Start](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.primitives/timerange/properties.md#start) is never after [TimeRange.End](/LaunchPad/pr-preview/pr-3323/api/rocket.surgery.launchpad.primitives/timerange/properties.md#end), so a range cannot run past midnight; an empty range has them equal.

<a id="constructor"></a>
<a id="constructor-timeonly-timeonly"></a>

## TimeRange(TimeOnly, TimeOnly)

- Name: `Constructor(TimeOnly, TimeOnly)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3323-merge.Sha.848d7df41fb82a23ce0738e64978a35c934d7558/src/Primitives/StrongTypes/TimeRange.cs#L19-L24)

Creates a range from `start` to `end`.

```csharp
public record TimeRange
{
    public TimeRange(
        TimeOnly start,
        TimeOnly end)
    {
        // ...
    }
}
```

## Parameters

- `start` (`TimeOnly`)
- `end` (`TimeOnly`)

## Exceptions

- `ArgumentException` -- `start` is after `end`.
