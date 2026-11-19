# HexColor Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3280/api/rocket.surgery.launchpad.primitives.md)
- Type: [HexColor](/LaunchPad/pr-preview/pr-3280/api/rocket.surgery.launchpad.primitives/hexcolor.md)
- Kind: `Methods`
- Members: `10`

A hexadecimal color, valid under [HexColorRules](/LaunchPad/pr-preview/pr-3280/api/rocket.surgery.launchpad.primitives/hexcolorrules.md).

<a id="asprimitive"></a>

## AsPrimitive

- Name: `AsPrimitive`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3280-merge.Sha.d53d78d7adee47674f27c3f3dda0796827a126dc/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_HexColor.g.cs#L54)

```csharp
public sealed class HexColor
{
    public string AsPrimitive()
    {
        // ...
    }
}
```

<a id="compareto"></a>
<a id="compareto-hexcolor"></a>

## CompareTo(HexColor?)

- Name: `CompareTo(HexColor?)`
- Returns: `int`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3280-merge.Sha.d53d78d7adee47674f27c3f3dda0796827a126dc/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_HexColor.g.cs#L148-L153)

```csharp
public sealed class HexColor
{
    public int CompareTo(
        HexColor? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([HexColor?](/LaunchPad/pr-preview/pr-3280/api/rocket.surgery.launchpad.primitives/hexcolor.md))

<a id="equals"></a>
<a id="equals-hexcolor"></a>

## Equals(HexColor?)

- Name: `Equals(HexColor?)`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3280-merge.Sha.d53d78d7adee47674f27c3f3dda0796827a126dc/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_HexColor.g.cs#L72)

```csharp
public sealed class HexColor
{
    public bool Equals(
        HexColor? other)
    {
        // ...
    }
}
```

## Parameters

- `other` ([HexColor?](/LaunchPad/pr-preview/pr-3280/api/rocket.surgery.launchpad.primitives/hexcolor.md))

<a id="equals-object"></a>

## Equals(object?)

- Name: `Equals(object?)`
- Modifiers: `override`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3280-merge.Sha.d53d78d7adee47674f27c3f3dda0796827a126dc/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_HexColor.g.cs#L107)

Determines whether the specified object is equal to the current object.

```csharp
public sealed class HexColor
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
- Returns: [HexColor](/LaunchPad/pr-preview/pr-3280/api/rocket.surgery.launchpad.primitives/hexcolor.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3280-merge.Sha.d53d78d7adee47674f27c3f3dda0796827a126dc/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_HexColor.g.cs#L23-L25)

```csharp
public sealed class HexColor
{
    public static HexColor From(
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
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3280-merge.Sha.d53d78d7adee47674f27c3f3dda0796827a126dc/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_HexColor.g.cs#L110)

Serves as the default hash function.

```csharp
public sealed class HexColor
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
<a id="op-equality-hexcolor-hexcolor"></a>

## op_Equality(HexColor?, HexColor?)

- Name: `op_Equality(HexColor?, HexColor?)`
- Modifiers: `static`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3280-merge.Sha.d53d78d7adee47674f27c3f3dda0796827a126dc/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_HexColor.g.cs#L83-L88)

```csharp
public sealed class HexColor
{
    public static bool operator ==(
        HexColor? left,
        HexColor? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([HexColor?](/LaunchPad/pr-preview/pr-3280/api/rocket.surgery.launchpad.primitives/hexcolor.md))
- `right` ([HexColor?](/LaunchPad/pr-preview/pr-3280/api/rocket.surgery.launchpad.primitives/hexcolor.md))

<a id="op-inequality"></a>
<a id="op-inequality-hexcolor-hexcolor"></a>

## op_Inequality(HexColor?, HexColor?)

- Name: `op_Inequality(HexColor?, HexColor?)`
- Modifiers: `static`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3280-merge.Sha.d53d78d7adee47674f27c3f3dda0796827a126dc/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_HexColor.g.cs#L98)

```csharp
public sealed class HexColor
{
    public static bool operator !=(
        HexColor? left,
        HexColor? right)
    {
        // ...
    }
}
```

## Parameters

- `left` ([HexColor?](/LaunchPad/pr-preview/pr-3280/api/rocket.surgery.launchpad.primitives/hexcolor.md))
- `right` ([HexColor?](/LaunchPad/pr-preview/pr-3280/api/rocket.surgery.launchpad.primitives/hexcolor.md))

<a id="tostring"></a>

## ToString

- Name: `ToString`
- Modifiers: `override`
- Returns: `string`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3280-merge.Sha.d53d78d7adee47674f27c3f3dda0796827a126dc/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_HexColor.g.cs#L116)

Returns a string that represents the current object.

```csharp
public sealed class HexColor
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
<a id="tryfrom-string-hexcolor"></a>

## TryFrom(string, HexColor)

- Name: `TryFrom(string, HexColor)`
- Modifiers: `static`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3280-merge.Sha.d53d78d7adee47674f27c3f3dda0796827a126dc/src/Primitives/obj/Release/net11.0/SuperStrong.Types.CodeAnalysis.Generators/SuperStrong.Types.CodeAnalysis.Generators.StrongTypeGenerator/Rocket_Surgery_LaunchPad_StrongTypes_HexColor.g.cs#L37-L44)

```csharp
public sealed class HexColor
{
    public static bool TryFrom(
        string value,
        out HexColor result)
    {
        // ...
    }
}
```

## Parameters

- `value` (`string`)
- `result` ([HexColor](/LaunchPad/pr-preview/pr-3280/api/rocket.surgery.launchpad.primitives/hexcolor.md))
