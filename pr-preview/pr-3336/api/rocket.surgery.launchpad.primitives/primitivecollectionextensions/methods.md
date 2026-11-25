# PrimitiveCollectionExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives.md)
- Type: [PrimitiveCollectionExtensions](/LaunchPad/pr-preview/pr-3336/api/rocket.surgery.launchpad.primitives/primitivecollectionextensions.md)
- Kind: `Methods`
- Members: `3`

Creates domain primitives from collections of values.

<a id="todomainprimitivearray"></a>
<a id="todomainprimitivearray-ienumerable-tvalue"></a>

## ToDomainPrimitiveArray(IEnumerable<TValue>)

- Name: `ToDomainPrimitiveArray(IEnumerable<TValue>)`
- Modifiers: `extension`
- Returns: `TPrimitive[]`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.7cda342c7a30094b955266549c4928a5c52b02bb/src/Primitives/PrimitiveCollectionExtensions.cs)

Creates a domain primitive from every value, or throws for the first value its rules reject, naming its index.

```csharp
public static class PrimitiveCollectionExtensions
{
    public static TPrimitive[] ToDomainPrimitiveArray<TPrimitive, TValue>(
        this IEnumerable<TValue> values)
    {
        // ...
    }
}
```

## Parameters

- `values` (`IEnumerable<TValue>`)
  The values.

## Returns

`TPrimitive[]` -- The domain primitives.

<a id="todomainprimitivearray-readonlyspan-tvalue"></a>

## ToDomainPrimitiveArray(ReadOnlySpan<TValue>)

- Name: `ToDomainPrimitiveArray(ReadOnlySpan<TValue>)`
- Modifiers: `extension`
- Returns: `TPrimitive[]`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.7cda342c7a30094b955266549c4928a5c52b02bb/src/Primitives/PrimitiveCollectionExtensions.cs)

Creates a domain primitive from every value, or throws for the first value its rules reject, naming its index.

```csharp
public static class PrimitiveCollectionExtensions
{
    public static TPrimitive[] ToDomainPrimitiveArray<TPrimitive, TValue>(
        this ReadOnlySpan<TValue> values)
    {
        // ...
    }
}
```

## Parameters

- `values` (`ReadOnlySpan<TValue>`)
  The values.

## Returns

`TPrimitive[]` -- The domain primitives.

<a id="todomainprimitivelist"></a>
<a id="todomainprimitivelist-ienumerable-tvalue"></a>

## ToDomainPrimitiveList(IEnumerable<TValue>)

- Name: `ToDomainPrimitiveList(IEnumerable<TValue>)`
- Modifiers: `extension`
- Returns: `List<TPrimitive>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3336-merge.Sha.7cda342c7a30094b955266549c4928a5c52b02bb/src/Primitives/PrimitiveCollectionExtensions.cs#L20-L28)

Creates a domain primitive from every value, or throws for the first value its rules reject, naming its index.

```csharp
public static class PrimitiveCollectionExtensions
{
    public static List<TPrimitive> ToDomainPrimitiveList<TPrimitive, TValue>(
        this IEnumerable<TValue> values)
    {
        // ...
    }
}
```

## Parameters

- `values` (`IEnumerable<TValue>`)
  The values.

## Returns

`List<TPrimitive>` -- The domain primitives.
