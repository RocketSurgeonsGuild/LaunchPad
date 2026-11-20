# UsefulEnumerableExtensions Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.foundation.md)
- Type: [UsefulEnumerableExtensions](/LaunchPad/pr-preview/pr-3302/api/rocket.surgery.launchpad.foundation/usefulenumerableextensions.md)
- Kind: `Methods`
- Members: `2`

Useful extensions for enumerations.

<a id="getasyncenumerator"></a>
<a id="getasyncenumerator-iasyncenumerator-t"></a>

## GetAsyncEnumerator(IAsyncEnumerator<T>)

- Name: `GetAsyncEnumerator(IAsyncEnumerator<T>)`
- Modifiers: `extension`
- Returns: `IAsyncEnumerator<T>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Foundation/Extensions/UsefulEnumerableExtensions.cs#L22)

Allows you to enumerate over an enumerator you have without the need for it to be an `Generic.IAsyncEnumerator`1`.

```csharp
public static class UsefulEnumerableExtensions
{
    public static IAsyncEnumerator<T> GetAsyncEnumerator<T>(
        this IAsyncEnumerator<T> enumerator)
    {
        // ...
    }
}
```

## Parameters

- `enumerator` (`IAsyncEnumerator<T>`)

<a id="getenumerator"></a>
<a id="getenumerator-ienumerator-t"></a>

## GetEnumerator(IEnumerator<T>)

- Name: `GetEnumerator(IEnumerator<T>)`
- Modifiers: `extension`
- Returns: `IEnumerator<T>`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3302-merge.Sha.ef1fddc80f2867bf510546b2a05239671f3f33c1/src/Foundation/Extensions/UsefulEnumerableExtensions.cs#L14)

Allows you to enumerate over an enumerator you have without the need for it to be an `Generic.IEnumerable`1`.

```csharp
public static class UsefulEnumerableExtensions
{
    public static IEnumerator<T> GetEnumerator<T>(
        this IEnumerator<T> enumerator)
    {
        // ...
    }
}
```

## Parameters

- `enumerator` (`IEnumerator<T>`)
