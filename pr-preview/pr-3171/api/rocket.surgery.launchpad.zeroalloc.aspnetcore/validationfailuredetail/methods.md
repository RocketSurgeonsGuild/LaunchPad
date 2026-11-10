# ValidationFailureDetail Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Type: [ValidationFailureDetail](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail.md)
- Kind: `Methods`
- Members: `8`

A single validation failure.

<a id="clone"></a>

## <Clone>$

- Name: `<Clone>$`
- Modifiers: `virtual`
- Returns: [ValidationFailureDetail](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail.md)

```csharp
public record ValidationFailureDetail
{
    public virtual ValidationFailureDetail <Clone>$()
    {
        // ...
    }
}
```

<a id="deconstruct"></a>
<a id="deconstruct-string-string-string"></a>

## Deconstruct(string, string, string?)

- Name: `Deconstruct(string, string, string?)`

```csharp
public record ValidationFailureDetail
{
    public void Deconstruct(
        out string PropertyName,
        out string ErrorMessage,
        out string? ErrorCode)
    {
        // ...
    }
}
```

## Parameters

- `PropertyName` (`string`)
- `ErrorMessage` (`string`)
- `ErrorCode` (`string?`)

<a id="equals"></a>
<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public record ValidationFailureDetail
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

<a id="equals-validationfailuredetail"></a>

## Equals(ValidationFailureDetail?)

- Name: `Equals(ValidationFailureDetail?)`
- Modifiers: `virtual`
- Returns: `bool`

Indicates whether the current object is equal to another object of the same type.

```csharp
public record ValidationFailureDetail
{
    public virtual bool Equals(
        ValidationFailureDetail? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([ValidationFailureDetail?](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail.md))
  An object to compare with this object.

## Returns

`bool` -- `true` if the current object is equal to the `other` parameter; otherwise, `false`.

<a id="gethashcode"></a>

## GetHashCode

- Name: `GetHashCode`
- Modifiers: `override`
- Returns: `int`

Serves as the default hash function.

```csharp
public record ValidationFailureDetail
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
<a id="op-equality-validationfailuredetail-validationfailuredetail"></a>

## op_Equality(ValidationFailureDetail?, ValidationFailureDetail?)

- Name: `op_Equality(ValidationFailureDetail?, ValidationFailureDetail?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record ValidationFailureDetail
{
    public static bool operator ==(
        ValidationFailureDetail? left,
        ValidationFailureDetail? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([ValidationFailureDetail?](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail.md))
- `right` ([ValidationFailureDetail?](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail.md))

<a id="op-inequality"></a>
<a id="op-inequality-validationfailuredetail-validationfailuredetail"></a>

## op_Inequality(ValidationFailureDetail?, ValidationFailureDetail?)

- Name: `op_Inequality(ValidationFailureDetail?, ValidationFailureDetail?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public record ValidationFailureDetail
{
    public static bool operator !=(
        ValidationFailureDetail? left,
        ValidationFailureDetail? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([ValidationFailureDetail?](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail.md))
- `right` ([ValidationFailureDetail?](/LaunchPad/pr-preview/pr-3171/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public record ValidationFailureDetail
{
    public override string ToString()
    {
        // ...
    }
}
```

## Returns

`string` -- A string that represents the current object.
