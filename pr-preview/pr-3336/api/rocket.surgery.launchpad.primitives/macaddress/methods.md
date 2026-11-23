# MacAddress Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives.md)
- Type: [MacAddress](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/macaddress.md)
- Kind: `Methods`
- Members: `10`

An EUI-48 MAC address, valid under [MacAddressRules](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/macaddressrules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.11f685f6c8c4739a5c9c37bdc0dec7baf7e3894e/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_MacAddress.g.cs#L54)

```csharp
public sealed class MacAddress
{
    public string AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-macaddress"></a>

## CompareTo(MacAddress?)

- Name: `CompareTo(MacAddress?)`
- Returns: `int`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.11f685f6c8c4739a5c9c37bdc0dec7baf7e3894e/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_MacAddress.g.cs#L148-L153)

```csharp
public sealed class MacAddress
{
    public int CompareTo(
        MacAddress? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([MacAddress?](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/macaddress.md))

<a id="equals"></a>
<a id="equals-macaddress"></a>

## Equals(MacAddress?)

- Name: `Equals(MacAddress?)`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.11f685f6c8c4739a5c9c37bdc0dec7baf7e3894e/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_MacAddress.g.cs#L72)

```csharp
public sealed class MacAddress
{
    public bool Equals(
        MacAddress? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([MacAddress?](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/macaddress.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.11f685f6c8c4739a5c9c37bdc0dec7baf7e3894e/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_MacAddress.g.cs#L107)

Determines whether the specified object is equal to the current object.

```csharp
public sealed class MacAddress
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
- Returns: [MacAddress](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/macaddress.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.11f685f6c8c4739a5c9c37bdc0dec7baf7e3894e/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_MacAddress.g.cs#L23-L25)

```csharp
public sealed class MacAddress
{
    public static MacAddress From(
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
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.11f685f6c8c4739a5c9c37bdc0dec7baf7e3894e/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_MacAddress.g.cs#L110)

Serves as the default hash function.

```csharp
public sealed class MacAddress
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
<a id="op-equality-macaddress-macaddress"></a>

## op_Equality(MacAddress?, MacAddress?)

- Name: `op_Equality(MacAddress?, MacAddress?)`
- Modifiers: `static`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.11f685f6c8c4739a5c9c37bdc0dec7baf7e3894e/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_MacAddress.g.cs#L83-L88)

```csharp
public sealed class MacAddress
{
    public static bool operator ==(
        MacAddress? left,
        MacAddress? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([MacAddress?](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/macaddress.md))
- `right` ([MacAddress?](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/macaddress.md))

<a id="op-inequality"></a>
<a id="op-inequality-macaddress-macaddress"></a>

## op_Inequality(MacAddress?, MacAddress?)

- Name: `op_Inequality(MacAddress?, MacAddress?)`
- Modifiers: `static`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.11f685f6c8c4739a5c9c37bdc0dec7baf7e3894e/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_MacAddress.g.cs#L98)

```csharp
public sealed class MacAddress
{
    public static bool operator !=(
        MacAddress? left,
        MacAddress? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([MacAddress?](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/macaddress.md))
- `right` ([MacAddress?](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/macaddress.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.11f685f6c8c4739a5c9c37bdc0dec7baf7e3894e/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_MacAddress.g.cs#L116)

Returns a string that represents the current object.

```csharp
public sealed class MacAddress
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
<a id="tryfrom-string-macaddress"></a>

## TryFrom(string, MacAddress)

- Name: `TryFrom(string, MacAddress)`
- Modifiers: `static`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.11f685f6c8c4739a5c9c37bdc0dec7baf7e3894e/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_MacAddress.g.cs#L37-L44)

```csharp
public sealed class MacAddress
{
    public static bool TryFrom(
        string value,
        out MacAddress result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`string`)
- `result` ([MacAddress](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/macaddress.md))
