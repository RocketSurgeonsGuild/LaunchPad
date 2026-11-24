# PrimitiveError Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives.md)
- Type: [PrimitiveError](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/primitiveerror.md)
- Kind: `Methods`
- Members: `8`

Why a domain primitive rejected a value: a code ( `LENGTH`, `FORMAT`, `RANGE`, ...) and a message.

<a id="create"></a>
<a id="create-string-string"></a>

## Create(string, string)

- Name: `Create(string, string)`
- Modifiers: `static`
- Returns: [PrimitiveError](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/primitiveerror.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3337-merge.Sha.c35dcc131245e369cef6bf89daaeca2d2d05743c/src/Primitives/PrimitiveError.cs#L27)

Creates an error.

```csharp
public struct PrimitiveError
{
    public static PrimitiveError Create(
        string code,
        string message)
    {
        // ...
    }
}
```

## Parameters

- `code` (`string`)
  The code.
- `message` (`string`)
  The message.

## Returns

[PrimitiveError](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/primitiveerror.md) -- The error.

<a id="deconstruct"></a>
<a id="deconstruct-string-string"></a>

## Deconstruct(string?, string?)

- Name: `Deconstruct(string?, string?)`

```csharp
public struct PrimitiveError
{
    public void Deconstruct(
        out string? Code,
        out string? Message)
    {
        // ...
    }
}
```

## Parameters

- `Code` (`string?`)
- `Message` (`string?`)

<a id="equals"></a>
<a id="equals-object"></a>

## Equals(object)

- Name: `Equals(object)`
- Modifiers: `override`
- Returns: `bool`

Indicates whether this instance and a specified object are equal.

```csharp
public struct PrimitiveError
{
    public override bool Equals(
        object obj)
    {
        // ...
    }
}
```

## Parameters

- `obj` (`object`)
  The object to compare with the current instance.

## Returns

`bool` -- `true` if `obj` and this instance are the same type and represent the same value; otherwise, `false`.

<a id="equals-primitiveerror"></a>

## Equals(PrimitiveError)

- Name: `Equals(PrimitiveError)`
- Returns: `bool`

Indicates whether the current object is equal to another object of the same type.

```csharp
public struct PrimitiveError
{
    public bool Equals(
        PrimitiveError other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([PrimitiveError](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/primitiveerror.md))
  An object to compare with this object.

## Returns

`bool` -- `true` if the current object is equal to the `other` parameter; otherwise, `false`.

<a id="gethashcode"></a>

## GetHashCode

- Name: `GetHashCode`
- Modifiers: `override`
- Returns: `int`

Returns the hash code for this instance.

```csharp
public struct PrimitiveError
{
    public override int GetHashCode()
    {
        // ...
    }
}
```

## Returns

`int` -- A 32-bit signed integer that is the hash code for this instance.

<a id="op-equality"></a>
<a id="op-equality-primitiveerror-primitiveerror"></a>

## op_Equality(PrimitiveError, PrimitiveError)

- Name: `op_Equality(PrimitiveError, PrimitiveError)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public struct PrimitiveError
{
    public static bool operator ==(
        PrimitiveError left,
        PrimitiveError right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([PrimitiveError](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/primitiveerror.md))
- `right` ([PrimitiveError](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/primitiveerror.md))

<a id="op-inequality"></a>
<a id="op-inequality-primitiveerror-primitiveerror"></a>

## op_Inequality(PrimitiveError, PrimitiveError)

- Name: `op_Inequality(PrimitiveError, PrimitiveError)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public struct PrimitiveError
{
    public static bool operator !=(
        PrimitiveError left,
        PrimitiveError right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([PrimitiveError](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/primitiveerror.md))
- `right` ([PrimitiveError](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.primitives/primitiveerror.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns the fully qualified type name of this instance.

```csharp
public struct PrimitiveError
{
    public override string ToString()
    {
        // ...
    }
}
```

## Returns

`string` -- The fully qualified type name.
