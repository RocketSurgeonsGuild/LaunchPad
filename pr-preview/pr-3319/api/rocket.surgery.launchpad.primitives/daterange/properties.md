# DateRange Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3319/api/rocket.surgery.launchpad.primitives.md)
- Type: [DateRange](/LaunchPad/pr-preview/pr-3319/api/rocket.surgery.launchpad.primitives/daterange.md)
- Kind: `Properties`
- Members: `2`

A range of dates from [DateRange.Start](/LaunchPad/pr-preview/pr-3319/api/rocket.surgery.launchpad.primitives/daterange/properties.md#start) to [DateRange.End](/LaunchPad/pr-preview/pr-3319/api/rocket.surgery.launchpad.primitives/daterange/properties.md#end), both included. [DateRange.Start](/LaunchPad/pr-preview/pr-3319/api/rocket.surgery.launchpad.primitives/daterange/properties.md#start) is never after [DateRange.End](/LaunchPad/pr-preview/pr-3319/api/rocket.surgery.launchpad.primitives/daterange/properties.md#end); a range of one day has them equal.

<a id="end"></a>

## End

- Name: `End`
- Modifiers: `get`
- Returns: `DateOnly`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3319-merge.Sha.db5ef919c4f6a03a7f33e4672dc70757d169d5a4/src/Primitives/StrongTypes/DateRange.cs)

The last day of the range.

```csharp
public DateOnly End { get; }
```

<a id="start"></a>

## Start

- Name: `Start`
- Modifiers: `get`
- Returns: `DateOnly`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3319-merge.Sha.db5ef919c4f6a03a7f33e4672dc70757d169d5a4/src/Primitives/StrongTypes/DateRange.cs)

The first day of the range.

```csharp
public DateOnly Start { get; }
```
