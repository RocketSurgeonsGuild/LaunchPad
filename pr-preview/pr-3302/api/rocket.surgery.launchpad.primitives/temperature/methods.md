# Temperature Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives.md)
- Type: [Temperature](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md)
- Kind: `Methods`
- Members: `23`

A temperature in degrees Celsius, valid under [TemperatureRules](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperaturerules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `double`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs#L54)

```csharp
public sealed class Temperature
{
    public double AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-temperature"></a>

## CompareTo(Temperature?)

- Name: `CompareTo(Temperature?)`
- Returns: `int`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs#L313-L318)

```csharp
public sealed class Temperature
{
    public int CompareTo(
        Temperature? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Temperature?](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md))

<a id="equals"></a>
<a id="equals-temperature"></a>

## Equals(Temperature?)

- Name: `Equals(Temperature?)`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs#L72)

```csharp
public sealed class Temperature
{
    public bool Equals(
        Temperature? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Temperature?](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs#L107)

Determines whether the specified object is equal to the current object.

```csharp
public sealed class Temperature
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
<a id="from-double"></a>

## From(double)

- Name: `From(double)`
- Modifiers: `static`
- Returns: [Temperature](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs#L23-L25)

```csharp
public sealed class Temperature
{
    public static Temperature From(
        double value)
    {
        // ...
    }
}
```

## Parameters

- `value` (`double`)

<a id="gethashcode"></a>

## GetHashCode

- Name: `GetHashCode`
- Modifiers: `override`
- Returns: `int`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs#L110)

Serves as the default hash function.

```csharp
public sealed class Temperature
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
<a id="op-equality-temperature-temperature"></a>

## op_Equality(Temperature?, Temperature?)

- Name: `op_Equality(Temperature?, Temperature?)`
- Modifiers: `static`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs#L83-L88)

```csharp
public sealed class Temperature
{
    public static bool operator ==(
        Temperature? left,
        Temperature? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Temperature?](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md))
- `right` ([Temperature?](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md))

<a id="op-greaterthan"></a>
<a id="op-greaterthan-temperature-temperature"></a>

## op_GreaterThan(Temperature, Temperature)

- Name: `op_GreaterThan(Temperature, Temperature)`
- Modifiers: `static`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs#L372-L375)

```csharp
public sealed class Temperature
{
    public static bool operator >(Temperature left, Temperature right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Temperature](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md))
- `right` ([Temperature](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md))

<a id="op-greaterthanorequal"></a>
<a id="op-greaterthanorequal-temperature-temperature"></a>

## op_GreaterThanOrEqual(Temperature, Temperature)

- Name: `op_GreaterThanOrEqual(Temperature, Temperature)`
- Modifiers: `static`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs#L389-L392)

```csharp
public sealed class Temperature
{
    public static bool operator >=(Temperature left, Temperature right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Temperature](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md))
- `right` ([Temperature](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md))

<a id="op-inequality"></a>
<a id="op-inequality-temperature-temperature"></a>

## op_Inequality(Temperature?, Temperature?)

- Name: `op_Inequality(Temperature?, Temperature?)`
- Modifiers: `static`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs#L98)

```csharp
public sealed class Temperature
{
    public static bool operator !=(
        Temperature? left,
        Temperature? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Temperature?](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md))
- `right` ([Temperature?](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md))

<a id="op-lessthan"></a>
<a id="op-lessthan-temperature-temperature"></a>

## op_LessThan(Temperature, Temperature)

- Name: `op_LessThan(Temperature, Temperature)`
- Modifiers: `static`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs#L338-L341)

```csharp
public sealed class Temperature
{
    public static bool operator <(Temperature left, Temperature right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Temperature](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md))
- `right` ([Temperature](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md))

<a id="op-lessthanorequal"></a>
<a id="op-lessthanorequal-temperature-temperature"></a>

## op_LessThanOrEqual(Temperature, Temperature)

- Name: `op_LessThanOrEqual(Temperature, Temperature)`
- Modifiers: `static`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs#L355-L358)

```csharp
public sealed class Temperature
{
    public static bool operator <=(Temperature left, Temperature right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Temperature](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md))
- `right` ([Temperature](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md))

<a id="parse"></a>
<a id="parse-string-iformatprovider"></a>

## Parse(string, IFormatProvider?)

- Name: `Parse(string, IFormatProvider?)`
- Modifiers: `static`
- Returns: [Temperature](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs)

```csharp
public sealed class Temperature
{
    public static Temperature Parse(
        string s,
        IFormatProvider? provider)
    {
        // ...
    }
}
```

## Parameters

- `s` (`string`)
- `provider` (`IFormatProvider?`)

<a id="parse-readonlyspan-char-iformatprovider"></a>

## Parse(ReadOnlySpan<char>, IFormatProvider?)

- Name: `Parse(ReadOnlySpan<char>, IFormatProvider?)`
- Modifiers: `static`
- Returns: [Temperature](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs)

```csharp
public sealed class Temperature
{
    public static Temperature Parse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider)
    {
        // ...
    }
}
```

## Parameters

- `s` (`ReadOnlySpan<char>`)
- `provider` (`IFormatProvider?`)

<a id="parse-readonlyspan-byte-iformatprovider"></a>

## Parse(ReadOnlySpan<byte>, IFormatProvider?)

- Name: `Parse(ReadOnlySpan<byte>, IFormatProvider?)`
- Modifiers: `static`
- Returns: [Temperature](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs#L210)

```csharp
public sealed class Temperature
{
    public static Temperature Parse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider)
    {
        // ...
    }
}
```

## Parameters

- `utf8Text` (`ReadOnlySpan<byte>`)
- `provider` (`IFormatProvider?`)

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs#L116)

Returns a string that represents the current object.

```csharp
public sealed class Temperature
{
    public override string ToString()
    {
        // ...
    }
}
```

## Returns

`string` -- A string that represents the current object.

<a id="tostring-string-iformatprovider"></a>

## ToString(string?, IFormatProvider?)

- Name: `ToString(string?, IFormatProvider?)`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs#L253)

```csharp
public sealed class Temperature
{
    public string ToString(
        string? format,
        IFormatProvider? formatProvider)
    {
        // ...
    }
}
```

## Parameters

- `format` (`string?`)
- `formatProvider` (`IFormatProvider?`)

<a id="tryformat"></a>
<a id="tryformat-span-char-int-readonlyspan-char-iformatprovider"></a>

## TryFormat(Span<char>, int, ReadOnlySpan<char>, IFormatProvider?)

- Name: `TryFormat(Span<char>, int, ReadOnlySpan<char>, IFormatProvider?)`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs#L273)

```csharp
public sealed class Temperature
{
    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        // ...
    }
}
```

## Parameters

- `destination` (`Span<char>`)
- `charsWritten` (`int`)
- `format` (`ReadOnlySpan<char>`)
- `provider` (`IFormatProvider?`)

<a id="tryformat-span-byte-int-readonlyspan-char-iformatprovider"></a>

## TryFormat(Span<byte>, int, ReadOnlySpan<char>, IFormatProvider?)

- Name: `TryFormat(Span<byte>, int, ReadOnlySpan<char>, IFormatProvider?)`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs#L293)

```csharp
public sealed class Temperature
{
    public bool TryFormat(
        Span<byte> utf8Destination,
        out int bytesWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        // ...
    }
}
```

## Parameters

- `utf8Destination` (`Span<byte>`)
- `bytesWritten` (`int`)
- `format` (`ReadOnlySpan<char>`)
- `provider` (`IFormatProvider?`)

<a id="tryfrom"></a>
<a id="tryfrom-double-temperature"></a>

## TryFrom(double, Temperature)

- Name: `TryFrom(double, Temperature)`
- Modifiers: `static`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs#L37-L44)

```csharp
public sealed class Temperature
{
    public static bool TryFrom(
        double value,
        out Temperature result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`double`)
- `result` ([Temperature](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md))

<a id="tryparse"></a>
<a id="tryparse-string-iformatprovider-temperature"></a>

## TryParse(string?, IFormatProvider?, Temperature)

- Name: `TryParse(string?, IFormatProvider?, Temperature)`
- Modifiers: `static`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs)

```csharp
public sealed class Temperature
{
    public static bool TryParse(
        string? s,
        IFormatProvider? provider,
        out Temperature result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`string?`)
- `provider` (`IFormatProvider?`)
- `result` ([Temperature](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md))

<a id="tryparse-readonlyspan-char-iformatprovider-temperature"></a>

## TryParse(ReadOnlySpan<char>, IFormatProvider?, Temperature)

- Name: `TryParse(ReadOnlySpan<char>, IFormatProvider?, Temperature)`
- Modifiers: `static`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs)

```csharp
public sealed class Temperature
{
    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        out Temperature result)
    {
        // ...
    }
}
```

## Parameters

- `s` (`ReadOnlySpan<char>`)
- `provider` (`IFormatProvider?`)
- `result` ([Temperature](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md))

<a id="tryparse-readonlyspan-byte-iformatprovider-temperature"></a>

## TryParse(ReadOnlySpan<byte>, IFormatProvider?, Temperature)

- Name: `TryParse(ReadOnlySpan<byte>, IFormatProvider?, Temperature)`
- Modifiers: `static`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Temperature.g.cs#L227-L233)

```csharp
public sealed class Temperature
{
    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider,
        out Temperature result)
    {
        // ...
    }
}
```

## Parameters

- `utf8Text` (`ReadOnlySpan<byte>`)
- `provider` (`IFormatProvider?`)
- `result` ([Temperature](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.primitives/temperature.md))
