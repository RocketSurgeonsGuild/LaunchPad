# TimeRange Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives.md)
- Type: [TimeRange](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/timerange.md)
- Kind: `Properties`
- Members: `3`

A range of times of day from [TimeRange.Start](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/timerange/properties.md#start) to [TimeRange.End](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/timerange/properties.md#end) within one day. [TimeRange.Start](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/timerange/properties.md#start) is never after [TimeRange.End](/LaunchPad/pr-preview/pr-3208/api/rocket.surgery.launchpad.primitives/timerange/properties.md#end), so a range cannot run past midnight; an empty range has them equal.

<a id="duration"></a>

## Duration

- Name: `Duration`
- Modifiers: `get`
- Returns: `TimeSpan`

How long the range is.

```csharp
public TimeSpan Duration { get; }
```

<a id="end"></a>

## End

- Name: `End`
- Modifiers: `get`
- Returns: `TimeOnly`

The time the range ends at.

```csharp
public TimeOnly End { get; }
```

<a id="start"></a>

## Start

- Name: `Start`
- Modifiers: `get`
- Returns: `TimeOnly`

The time the range starts at.

```csharp
public TimeOnly Start { get; }
```
