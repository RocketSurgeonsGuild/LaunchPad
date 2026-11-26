# PrimitiveBuilder<TPrimitive, TValue> Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3343/api/rocket.surgery.launchpad.primitives.md)
- Type: [PrimitiveBuilder<TPrimitive, TValue>](/LaunchPad/pr-preview/pr-3343/api/rocket.surgery.launchpad.primitives/primitivebuilder-2.md)
- Kind: `Methods`
- Members: `5`

Creates a domain primitive with rules of the caller's own on top of its rules.

<a id="build"></a>
<a id="build-tprimitive"></a>

## Build(TPrimitive?)

- Name: `Build(TPrimitive?)`
- Returns: `bool`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3343-merge.Sha.686f94dbc20c569566eba31b7d55404af5ea19ed/src/Primitives/PrimitiveBuilder.cs#L71-L79)

Creates the domain primitive, unless a rule rejects the value.

```csharp
public sealed class PrimitiveBuilder<TPrimitive, TValue>
{
    public bool PrimitiveBuilder<TPrimitive, Build(out TPrimitive? result)
    {
        // ...
    }
}
```

## Parameters

- `result` (`TPrimitive?`)
  The domain primitive.

## Returns

`bool` -- Whether the value was accepted.

<a id="buildorthrow"></a>

## BuildOrThrow

- Name: `BuildOrThrow`
- Returns: `TPrimitive`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3343-merge.Sha.686f94dbc20c569566eba31b7d55404af5ea19ed/src/Primitives/PrimitiveBuilder.cs#L55-L61)

Creates the domain primitive, or throws when a rule rejects the value.

```csharp
public sealed class PrimitiveBuilder<TPrimitive, TValue>
{
    public TPrimitive PrimitiveBuilder<TPrimitive, BuildOrThrow()
    {
        // ...
    }
}
```

## Returns

`TPrimitive` -- The domain primitive.

<a id="for"></a>

## For

- Name: `For`
- Modifiers: `static`
- Returns: [PrimitiveBuilder<TPrimitive, TValue>](/LaunchPad/pr-preview/pr-3343/api/rocket.surgery.launchpad.primitives/primitivebuilder-2.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3343-merge.Sha.686f94dbc20c569566eba31b7d55404af5ea19ed/src/Primitives/PrimitiveBuilder.cs#L20)

Starts a builder.

```csharp
public sealed class PrimitiveBuilder<TPrimitive, TValue>
{
    public static PrimitiveBuilder<TPrimitive, TValue> PrimitiveBuilder<TPrimitive, For()
    {
        // ...
    }
}
```

## Returns

[PrimitiveBuilder<TPrimitive, TValue>](/LaunchPad/pr-preview/pr-3343/api/rocket.surgery.launchpad.primitives/primitivebuilder-2.md) -- The builder.

<a id="must"></a>
<a id="must-func-tvalue-bool-string-string"></a>

## Must(Func<TValue, bool>, string, string)

- Name: `Must(Func<TValue, bool>, string, string)`
- Returns: [PrimitiveBuilder<TPrimitive, TValue>](/LaunchPad/pr-preview/pr-3343/api/rocket.surgery.launchpad.primitives/primitivebuilder-2.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3343-merge.Sha.686f94dbc20c569566eba31b7d55404af5ea19ed/src/Primitives/PrimitiveBuilder.cs#L44-L46)

Adds a rule.

```csharp
public sealed class PrimitiveBuilder<TPrimitive, TValue>
{
    public PrimitiveBuilder<TPrimitive, TValue> PrimitiveBuilder<TPrimitive, Must(Func<TValue, bool> predicate, string errorCode, string errorMessage)
    {
        // ...
    }
}
```

## Parameters

- `predicate` (`Func<TValue, bool>`)
  Whether the value is accepted.
- `errorCode` (`string`)
  The code when it is not.
- `errorMessage` (`string`)
  The message when it is not.

## Returns

[PrimitiveBuilder<TPrimitive, TValue>](/LaunchPad/pr-preview/pr-3343/api/rocket.surgery.launchpad.primitives/primitivebuilder-2.md) -- The builder.

<a id="withvalue"></a>
<a id="withvalue-tvalue"></a>

## WithValue(TValue)

- Name: `WithValue(TValue)`
- Returns: [PrimitiveBuilder<TPrimitive, TValue>](/LaunchPad/pr-preview/pr-3343/api/rocket.surgery.launchpad.primitives/primitivebuilder-2.md)
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3343-merge.Sha.686f94dbc20c569566eba31b7d55404af5ea19ed/src/Primitives/PrimitiveBuilder.cs#L31-L32)

Sets the value.

```csharp
public sealed class PrimitiveBuilder<TPrimitive, TValue>
{
    public PrimitiveBuilder<TPrimitive, TValue> PrimitiveBuilder<TPrimitive, WithValue(TValue value)
    {
        // ...
    }
}
```

## Parameters

- `value` (`TValue`)
  The value.

## Returns

[PrimitiveBuilder<TPrimitive, TValue>](/LaunchPad/pr-preview/pr-3343/api/rocket.surgery.launchpad.primitives/primitivebuilder-2.md) -- The builder.
