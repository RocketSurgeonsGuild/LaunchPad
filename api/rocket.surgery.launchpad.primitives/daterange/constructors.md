# DateRange Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [DateRange](/LaunchPad/api/rocket.surgery.launchpad.primitives/daterange.md)
- Kind: `Constructors`
- Members: `1`

A range of dates from [DateRange.Start](/LaunchPad/api/rocket.surgery.launchpad.primitives/daterange/properties.md#start) to [DateRange.End](/LaunchPad/api/rocket.surgery.launchpad.primitives/daterange/properties.md#end), both included. [DateRange.Start](/LaunchPad/api/rocket.surgery.launchpad.primitives/daterange/properties.md#start) is never after [DateRange.End](/LaunchPad/api/rocket.surgery.launchpad.primitives/daterange/properties.md#end); a range of one day has them equal.

<a id="constructor"></a>
<a id="constructor-dateonly-dateonly"></a>

## DateRange(DateOnly, DateOnly)

- Name: `Constructor(DateOnly, DateOnly)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.974bfd9a73f64517e8a81d86b29393f3cce75778/src/Primitives/StrongTypes/DateRange.cs#L19-L24)

Creates a range from `start` to `end`.

```csharp
public record DateRange
{
    public DateRange(
        DateOnly start,
        DateOnly end)
    {
        // ...
    }
}
```

## Parameters

- `start` (`DateOnly`)
- `end` (`DateOnly`)

## Exceptions

- `ArgumentException` -- `start` is after `end`.
