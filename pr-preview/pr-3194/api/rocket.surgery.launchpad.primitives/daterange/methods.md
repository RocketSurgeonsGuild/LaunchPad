# DateRange Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives.md)
- Type: [DateRange](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/daterange.md)
- Kind: `Methods`
- Members: `10`

A range of dates from [DateRange.Start](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/daterange/properties.md#start) to [DateRange.End](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/daterange/properties.md#end), both included. [DateRange.Start](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/daterange/properties.md#start) is never after [DateRange.End](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/daterange/properties.md#end); a range of one day has them equal.

<a id="clone"></a>

## <Clone>$

- Name: `<Clone>$`
- Returns: [DateRange](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/daterange.md)

```csharp
public record DateRange
{
    public DateRange <Clone>$()
    {
        // ...
    }
}
```

<a id="contains"></a>
<a id="contains-dateonly"></a>

## Contains(DateOnly)

- Name: `Contains(DateOnly)`
- Returns: `bool`

Whether `date` is in the range.

```csharp
public record DateRange
{
    public bool Contains(
        DateOnly date)
    {
        // ...
    }
}
```

## Parameters

- `date` (`DateOnly`)

<a id="equals"></a>
<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public record DateRange
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

<a id="equals-daterange"></a>

## Equals(DateRange?)

- Name: `Equals(DateRange?)`
- Returns: `bool`

Indicates whether the current object is equal to another object of the same type.

```csharp
public record DateRange
{
    public bool Equals(
        DateRange? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([DateRange?](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/daterange.md))
  An object to compare with this object.

## Returns

`bool` -- `true` if the current object is equal to the `other` parameter; otherwise, `false`.

<a id="from"></a>
<a id="from-dateonly-dateonly"></a>

## From(DateOnly, DateOnly)

- Name: `From(DateOnly, DateOnly)`
- Modifiers: `static`
- Returns: [DateRange](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/daterange.md)

Creates a range from `start` to `end`.

```csharp
public record DateRange
{
    public static DateRange From(
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

<a id="gethashcode"></a>

## GetHashCode

- Name: `GetHashCode`
- Modifiers: `override`
- Returns: `int`

Serves as the default hash function.

```csharp
public record DateRange
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
<a id="op-equality-daterange-daterange"></a>

## op_Equality(DateRange?, DateRange?)

- Name: `op_Equality(DateRange?, DateRange?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record DateRange
{
    public static bool operator ==(
        DateRange? left,
        DateRange? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([DateRange?](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/daterange.md))
- `right` ([DateRange?](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/daterange.md))

<a id="op-inequality"></a>
<a id="op-inequality-daterange-daterange"></a>

## op_Inequality(DateRange?, DateRange?)

- Name: `op_Inequality(DateRange?, DateRange?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record DateRange
{
    public static bool operator !=(
        DateRange? left,
        DateRange? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([DateRange?](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/daterange.md))
- `right` ([DateRange?](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/daterange.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public record DateRange
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
<a id="tryfrom-dateonly-dateonly-daterange"></a>

## TryFrom(DateOnly, DateOnly, DateRange?)

- Name: `TryFrom(DateOnly, DateOnly, DateRange?)`
- Modifiers: `static`
- Returns: `bool`

Creates a range from `start` to `end`, or returns `false` if it is invalid.

```csharp
public record DateRange
{
    public static bool TryFrom(
        DateOnly start,
        DateOnly end,
        out DateRange? result)
    {
        // ...
    }
}
```

## Parameters

- `start` (`DateOnly`)
- `end` (`DateOnly`)
- `result` ([DateRange?](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/daterange.md))
