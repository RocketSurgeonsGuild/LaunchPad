# TimeRange Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [TimeRange](/LaunchPad/api/rocket.surgery.launchpad.primitives/timerange.md)
- Kind: `Constructors`
- Members: `1`

A range of times of day from [TimeRange.Start](/LaunchPad/api/rocket.surgery.launchpad.primitives/timerange/properties.md#start) to [TimeRange.End](/LaunchPad/api/rocket.surgery.launchpad.primitives/timerange/properties.md#end) within one day. [TimeRange.Start](/LaunchPad/api/rocket.surgery.launchpad.primitives/timerange/properties.md#start) is never after [TimeRange.End](/LaunchPad/api/rocket.surgery.launchpad.primitives/timerange/properties.md#end), so a range cannot run past midnight; an empty range has them equal.

<a id="constructor"></a>
<a id="constructor-timeonly-timeonly"></a>

## TimeRange(TimeOnly, TimeOnly)

- Name: `Constructor(TimeOnly, TimeOnly)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.be4f65663b97e5bb7c221e0ce4f3da85894aeb85/src/Primitives/StrongTypes/TimeRange.cs#L19-L24)

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
