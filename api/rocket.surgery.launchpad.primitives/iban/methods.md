# Iban Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Type: [Iban](/LaunchPad/api/rocket.surgery.launchpad.primitives/iban.md)
- Kind: `Methods`
- Members: `10`

An IBAN in electronic format, valid under [IbanRules](/LaunchPad/api/rocket.surgery.launchpad.primitives/ibanrules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.58a05033e5e3d052ad687f1ec7ee0750304f817e/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Iban.g.cs#L54)

```csharp
public sealed class Iban
{
    public string AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-iban"></a>

## CompareTo(Iban?)

- Name: `CompareTo(Iban?)`
- Returns: `int`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.58a05033e5e3d052ad687f1ec7ee0750304f817e/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Iban.g.cs#L148-L153)

```csharp
public sealed class Iban
{
    public int CompareTo(
        Iban? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Iban?](/LaunchPad/api/rocket.surgery.launchpad.primitives/iban.md))

<a id="equals"></a>
<a id="equals-iban"></a>

## Equals(Iban?)

- Name: `Equals(Iban?)`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.58a05033e5e3d052ad687f1ec7ee0750304f817e/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Iban.g.cs#L72)

```csharp
public sealed class Iban
{
    public bool Equals(
        Iban? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([Iban?](/LaunchPad/api/rocket.surgery.launchpad.primitives/iban.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.58a05033e5e3d052ad687f1ec7ee0750304f817e/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Iban.g.cs#L107)

Determines whether the specified object is equal to the current object.

```csharp
public sealed class Iban
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
- Returns: [Iban](/LaunchPad/api/rocket.surgery.launchpad.primitives/iban.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.58a05033e5e3d052ad687f1ec7ee0750304f817e/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Iban.g.cs#L23-L25)

```csharp
public sealed class Iban
{
    public static Iban From(
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
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.58a05033e5e3d052ad687f1ec7ee0750304f817e/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Iban.g.cs#L110)

Serves as the default hash function.

```csharp
public sealed class Iban
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
<a id="op-equality-iban-iban"></a>

## op_Equality(Iban?, Iban?)

- Name: `op_Equality(Iban?, Iban?)`
- Modifiers: `static`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.58a05033e5e3d052ad687f1ec7ee0750304f817e/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Iban.g.cs#L83-L88)

```csharp
public sealed class Iban
{
    public static bool operator ==(
        Iban? left,
        Iban? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Iban?](/LaunchPad/api/rocket.surgery.launchpad.primitives/iban.md))
- `right` ([Iban?](/LaunchPad/api/rocket.surgery.launchpad.primitives/iban.md))

<a id="op-inequality"></a>
<a id="op-inequality-iban-iban"></a>

## op_Inequality(Iban?, Iban?)

- Name: `op_Inequality(Iban?, Iban?)`
- Modifiers: `static`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.58a05033e5e3d052ad687f1ec7ee0750304f817e/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Iban.g.cs#L98)

```csharp
public sealed class Iban
{
    public static bool operator !=(
        Iban? left,
        Iban? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([Iban?](/LaunchPad/api/rocket.surgery.launchpad.primitives/iban.md))
- `right` ([Iban?](/LaunchPad/api/rocket.surgery.launchpad.primitives/iban.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.58a05033e5e3d052ad687f1ec7ee0750304f817e/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Iban.g.cs#L116)

Returns a string that represents the current object.

```csharp
public sealed class Iban
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
<a id="tryfrom-string-iban"></a>

## TryFrom(string, Iban)

- Name: `TryFrom(string, Iban)`
- Modifiers: `static`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.58a05033e5e3d052ad687f1ec7ee0750304f817e/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_Iban.g.cs#L37-L44)

```csharp
public sealed class Iban
{
    public static bool TryFrom(
        string value,
        out Iban result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`string`)
- `result` ([Iban](/LaunchPad/api/rocket.surgery.launchpad.primitives/iban.md))
