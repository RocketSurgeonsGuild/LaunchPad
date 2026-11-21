# TimeRange Properties

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [TimeRange](/LaunchPad/api/rocket.surgery.launchpad.primitives/timerange.md)
- Kind: `Properties`
- Members: `3`

A range of times of day from [TimeRange.Start](/LaunchPad/api/rocket.surgery.launchpad.primitives/timerange/properties.md#start) to [TimeRange.End](/LaunchPad/api/rocket.surgery.launchpad.primitives/timerange/properties.md#end) within one day. [TimeRange.Start](/LaunchPad/api/rocket.surgery.launchpad.primitives/timerange/properties.md#start) is never after [TimeRange.End](/LaunchPad/api/rocket.surgery.launchpad.primitives/timerange/properties.md#end), so a range cannot run past midnight; an empty range has them equal.

<a id="duration"></a>

## Duration

- Name: `Duration`
- Modifiers: `get`
- Returns: `TimeSpan`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.be4f65663b97e5bb7c221e0ce4f3da85894aeb85/src/Primitives/StrongTypes/TimeRange.cs#L45)

How long the range is.

```csharp
public TimeSpan Duration { get; }
```

<a id="end"></a>

## End

- Name: `End`
- Modifiers: `get`
- Returns: `TimeOnly`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.be4f65663b97e5bb7c221e0ce4f3da85894aeb85/src/Primitives/StrongTypes/TimeRange.cs)

The time the range ends at.

```csharp
public TimeOnly End { get; }
```

<a id="start"></a>

## Start

- Name: `Start`
- Modifiers: `get`
- Returns: `TimeOnly`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.be4f65663b97e5bb7c221e0ce4f3da85894aeb85/src/Primitives/StrongTypes/TimeRange.cs)

The time the range starts at.

```csharp
public TimeOnly Start { get; }
```
