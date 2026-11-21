# UsefulEnumerableExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1270`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation.Extensions`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3317-merge.Sha.1621484e9867a9384634a5b693a135763b433b4a/src/Foundation/Extensions/UsefulEnumerableExtensions.cs)

Useful extensions for enumerations.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation.Extensions;

public static class UsefulEnumerableExtensions
{
    // ...
}
```

## Methods

- [GetAsyncEnumerator(IAsyncEnumerator<T>)](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.foundation/usefulenumerableextensions/methods.md#getasyncenumerator-iasyncenumerator-t) : `IAsyncEnumerator<T>` `extension` -- Allows you to enumerate over an enumerator you have without the need for it to be an `Generic.IAsyncEnumerator`1`.
- [GetEnumerator(IEnumerator<T>)](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.foundation/usefulenumerableextensions/methods.md#getenumerator-ienumerator-t) : `IEnumerator<T>` `extension` -- Allows you to enumerate over an enumerator you have without the need for it to be an `Generic.IEnumerable`1`.
