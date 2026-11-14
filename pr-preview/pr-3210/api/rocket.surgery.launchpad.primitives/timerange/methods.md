# TimeRange Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives.md)
- Type: [TimeRange](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/timerange.md)
- Kind: `Methods`
- Members: `9`

A range of times of day from [TimeRange.Start](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/timerange/properties.md#start) to [TimeRange.End](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/timerange/properties.md#end) within one day. [TimeRange.Start](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/timerange/properties.md#start) is never after [TimeRange.End](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/timerange/properties.md#end), so a range cannot run past midnight; an empty range has them equal.

<a id="clone"></a>

## <Clone>$

- Name: `<Clone>$`
- Returns: [TimeRange](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/timerange.md)

```csharp
public record TimeRange
{
    public TimeRange <Clone>$()
    {
        // ...
    }
}
```

<a id="equals"></a>
<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public record TimeRange
{
    public override bool Equals(
        object? obj)
    {
        // ...
    }
}
```

## Parameters

- `obj` (`object?`)
  The object to compare with the current object.

## Returns

`bool` -- `true` if the specified object is equal to the current object; otherwise, `false`.

<a id="equals-timerange"></a>

## Equals(TimeRange?)

- Name: `Equals(TimeRange?)`
- Returns: `bool`

Indicates whether the current object is equal to another object of the same type.

```csharp
public record TimeRange
{
    public bool Equals(
        TimeRange? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([TimeRange?](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/timerange.md))
  An object to compare with this object.

## Returns

`bool` -- `true` if the current object is equal to the `other` parameter; otherwise, `false`.

<a id="from"></a>
<a id="from-timeonly-timeonly"></a>

## From(TimeOnly, TimeOnly)

- Name: `From(TimeOnly, TimeOnly)`
- Modifiers: `static`
- Returns: [TimeRange](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/timerange.md)

Creates a range from `start` to `end`.

```csharp
public record TimeRange
{
    public static TimeRange From(
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

<a id="gethashcode"></a>

## GetHashCode

- Name: `GetHashCode`
- Modifiers: `override`
- Returns: `int`

Serves as the default hash function.

```csharp
public record TimeRange
{
    public override int GetHashCode()
    {
        // ...
    }
}
```

## Returns

`int` -- A hash code for the current object.

<a id="op-equality"></a>
<a id="op-equality-timerange-timerange"></a>

## op_Equality(TimeRange?, TimeRange?)

- Name: `op_Equality(TimeRange?, TimeRange?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record TimeRange
{
    public static bool operator ==(
        TimeRange? left,
        TimeRange? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([TimeRange?](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/timerange.md))
- `right` ([TimeRange?](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/timerange.md))

<a id="op-inequality"></a>
<a id="op-inequality-timerange-timerange"></a>

## op_Inequality(TimeRange?, TimeRange?)

- Name: `op_Inequality(TimeRange?, TimeRange?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record TimeRange
{
    public static bool operator !=(
        TimeRange? left,
        TimeRange? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([TimeRange?](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/timerange.md))
- `right` ([TimeRange?](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/timerange.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public record TimeRange
{
    public override string ToString()
    {
        // ...
    }
}
```

## Returns

`string` -- A string that represents the current object.

<a id="tryfrom"></a>
<a id="tryfrom-timeonly-timeonly-timerange"></a>

## TryFrom(TimeOnly, TimeOnly, TimeRange?)

- Name: `TryFrom(TimeOnly, TimeOnly, TimeRange?)`
- Modifiers: `static`
- Returns: `bool`

Creates a range from `start` to `end`, or returns `false` if it is invalid.

```csharp
public record TimeRange
{
    public static bool TryFrom(
        TimeOnly start,
        TimeOnly end,
        out TimeRange? result)
    {
        // ...
    }
}
```

## Parameters

- `start` (`TimeOnly`)
- `end` (`TimeOnly`)
- `result` ([TimeRange?](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.primitives/timerange.md))
