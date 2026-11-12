# Url Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives.md)
- Type: [Url](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/url.md)
- Kind: `Methods`
- Members: `10`

An absolute http or https URL, valid under [UrlRules](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/urlrules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `string`

```csharp
public sealed class Url
{
    public string AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-url"></a>

## CompareTo(Url?)

- Name: `CompareTo(Url?)`
- Returns: `int`

```csharp
public sealed class Url
{
    public int CompareTo(
        Url? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Url?](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/url.md))

<a id="equals"></a>
<a id="equals-url"></a>

## Equals(Url?)

- Name: `Equals(Url?)`
- Returns: `bool`

```csharp
public sealed class Url
{
    public bool Equals(
        Url? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Url?](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/url.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class Url
{
    public sealed override bool Equals(
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

<a id="from"></a>
<a id="from-string"></a>

## From(string)

- Name: `From(string)`
- Modifiers: `static`
- Returns: [Url](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/url.md)

```csharp
public sealed class Url
{
    public static Url From(
        string value)
    {
        // ...
    }
}
```

## Parameters

- `value` (`string`)

<a id="gethashcode"></a>

## GetHashCode

- Name: `GetHashCode`
- Modifiers: `override`
- Returns: `int`

Serves as the default hash function.

```csharp
public sealed class Url
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
<a id="op-equality-url-url"></a>

## op_Equality(Url?, Url?)

- Name: `op_Equality(Url?, Url?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Url
{
    public static bool operator ==(
        Url? left,
        Url? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Url?](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/url.md))
- `right` ([Url?](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/url.md))

<a id="op-inequality"></a>
<a id="op-inequality-url-url"></a>

## op_Inequality(Url?, Url?)

- Name: `op_Inequality(Url?, Url?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Url
{
    public static bool operator !=(
        Url? left,
        Url? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Url?](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/url.md))
- `right` ([Url?](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/url.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public sealed class Url
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
<a id="tryfrom-string-url"></a>

## TryFrom(string, Url)

- Name: `TryFrom(string, Url)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class Url
{
    public static bool TryFrom(
        string value,
        out Url result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`string`)
- `result` ([Url](/LaunchPad/pr-preview/pr-3194/api/rocket.surgery.launchpad.primitives/url.md))
