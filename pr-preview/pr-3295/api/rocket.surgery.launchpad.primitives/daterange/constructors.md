# DateRange Constructors

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3295/api/rocket.surgery.launchpad.primitives.md)
- Type: [DateRange](/LaunchPad/pr-preview/pr-3295/api/rocket.surgery.launchpad.primitives/daterange.md)
- Kind: `Constructors`
- Members: `1`

A range of dates from [DateRange.Start](/LaunchPad/pr-preview/pr-3295/api/rocket.surgery.launchpad.primitives/daterange/properties.md#start) to [DateRange.End](/LaunchPad/pr-preview/pr-3295/api/rocket.surgery.launchpad.primitives/daterange/properties.md#end), both included. [DateRange.Start](/LaunchPad/pr-preview/pr-3295/api/rocket.surgery.launchpad.primitives/daterange/properties.md#start) is never after [DateRange.End](/LaunchPad/pr-preview/pr-3295/api/rocket.surgery.launchpad.primitives/daterange/properties.md#end); a range of one day has them equal.

<a id="constructor"></a>
<a id="constructor-dateonly-dateonly"></a>

## DateRange(DateOnly, DateOnly)

- Name: `Constructor(DateOnly, DateOnly)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3295-merge.Sha.8032622be33212edca749d51eeb2f284c76d51b7/src/Primitives/StrongTypes/DateRange.cs#L19-L24)

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
