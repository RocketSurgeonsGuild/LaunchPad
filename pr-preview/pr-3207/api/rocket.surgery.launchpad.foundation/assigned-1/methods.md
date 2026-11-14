# Assigned<T> Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation.md)
- Type: [Assigned<T>](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation/assigned-1.md)
- Kind: `Methods`
- Members: `9`

The Assigned type is used to differentiate between not set and set input values.

<a id="empty"></a>
<a id="empty-t"></a>

## Empty(T?)

- Name: `Empty(T?)`
- Modifiers: `static`
- Returns: [Assigned<T>](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation/assigned-1.md)

Creates an empty Assigned that provides a default value.

```csharp
public class Assigned<T>
{
    public static Assigned<T> Empty(
        T? defaultValue = default(T?))
    {
        // ...
    }
}
```

## Parameters

- `defaultValue` (`T?`) `optional`
  The default value.

<a id="equals"></a>
<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Compares this `Foundation.Assigned`1` value to another value.

```csharp
public class Assigned<T>
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
  The second `Foundation.Assigned`1` for comparison.

## Returns

`bool` -- `true` if both `Foundation.Assigned`1` values are equal.

<a id="equals-assigned-t"></a>

## Equals(Assigned<T>)

- Name: `Equals(Assigned<T>)`
- Returns: `bool`

Compares this `Foundation.Assigned`1` value to another value.

```csharp
public class Assigned<T>
{
    public bool Equals(
        Assigned<T>? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Assigned<T>](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation/assigned-1.md))
  The second `Foundation.Assigned`1` for comparison.

## Returns

`bool` -- `true` if both `Foundation.Assigned`1` values are equal.

<a id="gethashcode"></a>

## GetHashCode

- Name: `GetHashCode`
- Modifiers: `override`
- Returns: `int`

Serves as a hash function for a `Foundation.Assigned`1` object.

```csharp
public class Assigned<T>
{
    public override int GetHashCode()
    {
        // ...
    }
}
```

## Returns

`int` -- A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.

<a id="op-equality"></a>
<a id="op-equality-assigned-t-assigned-t"></a>

## op_Equality(Assigned<T>, Assigned<T>)

- Name: `op_Equality(Assigned<T>, Assigned<T>)`
- Modifiers: `static`
- Returns: `bool`

Operator call through to Equals

```csharp
public class Assigned<T>
{
    public static bool operator ==(
        Assigned<T> left,
        Assigned<T> right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Assigned<T>](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation/assigned-1.md))
  The left parameter
- `right` ([Assigned<T>](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation/assigned-1.md))
  The right parameter

## Returns

`bool` -- `true` if both `Foundation.Assigned`1` values are equal.

<a id="op-implicit"></a>
<a id="op-implicit-t"></a>

## op_Implicit(T)

- Name: `op_Implicit(T)`
- Modifiers: `static`
- Returns: [Assigned<T>](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation/assigned-1.md)

Implicitly creates a new `Foundation.Assigned`1` from the given value.

```csharp
public class Assigned<T>
{
    public static implicit operator Assigned<T>(
        T value)
    {
        // ...
    }
}
```

## Parameters

- `value` (`T`)
  The value.

<a id="op-implicit-assigned-t"></a>

## op_Implicit(Assigned<T>)

- Name: `op_Implicit(Assigned<T>)`
- Modifiers: `static` `nullable`
- Returns: `T?`

Implicitly gets the Assigned value.

```csharp
public class Assigned<T>
{
    public static implicit operator T?(
        Assigned<T> assigned)
    {
        // ...
    }
}
```

## Parameters

- `assigned` ([Assigned<T>](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation/assigned-1.md))

<a id="op-inequality"></a>
<a id="op-inequality-assigned-t-assigned-t"></a>

## op_Inequality(Assigned<T>, Assigned<T>)

- Name: `op_Inequality(Assigned<T>, Assigned<T>)`
- Modifiers: `static`
- Returns: `bool`

Operator call through to Equals

```csharp
public class Assigned<T>
{
    public static bool operator !=(
        Assigned<T> left,
        Assigned<T> right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Assigned<T>](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation/assigned-1.md))
  The left parameter
- `right` ([Assigned<T>](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.foundation/assigned-1.md))
  The right parameter

## Returns

`bool` -- `true` if both `Foundation.Assigned`1` values are not equal.

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Provides the name string.

```csharp
public class Assigned<T>
{
    public override string ToString()
    {
        // ...
    }
}
```

## Returns

`string` -- The name string value
