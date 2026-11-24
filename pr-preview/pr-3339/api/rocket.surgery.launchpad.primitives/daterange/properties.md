# DateRange Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3339/api/rocket.surgery.launchpad.primitives.md)
- Type: [DateRange](/LaunchPad/pr-preview/pr-3339/api/rocket.surgery.launchpad.primitives/daterange.md)
- Kind: `Properties`
- Members: `2`

A range of dates from [DateRange.Start](/LaunchPad/pr-preview/pr-3339/api/rocket.surgery.launchpad.primitives/daterange/properties.md#start) to [DateRange.End](/LaunchPad/pr-preview/pr-3339/api/rocket.surgery.launchpad.primitives/daterange/properties.md#end), both included. [DateRange.Start](/LaunchPad/pr-preview/pr-3339/api/rocket.surgery.launchpad.primitives/daterange/properties.md#start) is never after [DateRange.End](/LaunchPad/pr-preview/pr-3339/api/rocket.surgery.launchpad.primitives/daterange/properties.md#end); a range of one day has them equal.

<a id="end"></a>

## End

- Name: `End`
- Modifiers: `get`
- Returns: `DateOnly`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3339-merge.Sha.f2b0430cc034379f71251d40ca4b57fa907c264f/src/Primitives/StrongTypes/DateRange.cs)

The last day of the range.

```csharp
public DateOnly End { get; }
```

<a id="start"></a>

## Start

- Name: `Start`
- Modifiers: `get`
- Returns: `DateOnly`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3339-merge.Sha.f2b0430cc034379f71251d40ca4b57fa907c264f/src/Primitives/StrongTypes/DateRange.cs)

The first day of the range.

```csharp
public DateOnly Start { get; }
```
