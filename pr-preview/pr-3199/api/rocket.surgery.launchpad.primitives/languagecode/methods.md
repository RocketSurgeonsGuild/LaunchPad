# LanguageCode Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives.md)
- Type: [LanguageCode](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/languagecode.md)
- Kind: `Methods`
- Members: `10`

A BCP 47 language tag, valid under [LanguageCodeRules](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/languagecoderules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `string`

```csharp
public sealed class LanguageCode
{
    public string AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-languagecode"></a>

## CompareTo(LanguageCode?)

- Name: `CompareTo(LanguageCode?)`
- Returns: `int`

```csharp
public sealed class LanguageCode
{
    public int CompareTo(
        LanguageCode? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([LanguageCode?](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/languagecode.md))

<a id="equals"></a>
<a id="equals-languagecode"></a>

## Equals(LanguageCode?)

- Name: `Equals(LanguageCode?)`
- Returns: `bool`

```csharp
public sealed class LanguageCode
{
    public bool Equals(
        LanguageCode? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([LanguageCode?](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/languagecode.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`

Determines whether the specified object is equal to the current object.

```csharp
public sealed class LanguageCode
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
- Returns: [LanguageCode](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/languagecode.md)

```csharp
public sealed class LanguageCode
{
    public static LanguageCode From(
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
public sealed class LanguageCode
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
<a id="op-equality-languagecode-languagecode"></a>

## op_Equality(LanguageCode?, LanguageCode?)

- Name: `op_Equality(LanguageCode?, LanguageCode?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class LanguageCode
{
    public static bool operator ==(
        LanguageCode? left,
        LanguageCode? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([LanguageCode?](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/languagecode.md))
- `right` ([LanguageCode?](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/languagecode.md))

<a id="op-inequality"></a>
<a id="op-inequality-languagecode-languagecode"></a>

## op_Inequality(LanguageCode?, LanguageCode?)

- Name: `op_Inequality(LanguageCode?, LanguageCode?)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class LanguageCode
{
    public static bool operator !=(
        LanguageCode? left,
        LanguageCode? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([LanguageCode?](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/languagecode.md))
- `right` ([LanguageCode?](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/languagecode.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`

Returns a string that represents the current object.

```csharp
public sealed class LanguageCode
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
<a id="tryfrom-string-languagecode"></a>

## TryFrom(string, LanguageCode)

- Name: `TryFrom(string, LanguageCode)`
- Modifiers: `static`
- Returns: `bool`

```csharp
public sealed class LanguageCode
{
    public static bool TryFrom(
        string value,
        out LanguageCode result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`string`)
- `result` ([LanguageCode](/LaunchPad/pr-preview/pr-3199/api/rocket.surgery.launchpad.primitives/languagecode.md))
