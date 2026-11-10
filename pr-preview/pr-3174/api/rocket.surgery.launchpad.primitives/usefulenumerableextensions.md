# UsefulEnumerableExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1165`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives.Extensions`
- Target framework: `net10.0`

Useful extensions for enumerations.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives.Extensions;

public static class UsefulEnumerableExtensions
{
    // ...
}
```

## Methods

- [GetAsyncEnumerator(IAsyncEnumerator<T>)](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/usefulenumerableextensions/methods.md#getasyncenumerator-iasyncenumerator-t) : `IAsyncEnumerator<T>` `extension` -- Allows you to enumerate over an enumerator you have without the need for it to be an `Generic.IAsyncEnumerator`1`.
- [GetEnumerator(IEnumerator<T>)](/LaunchPad/pr-preview/pr-3174/api/rocket.surgery.launchpad.primitives/usefulenumerableextensions/methods.md#getenumerator-ienumerator-t) : `IEnumerator<T>` `extension` -- Allows you to enumerate over an enumerator you have without the need for it to be an `Generic.IEnumerable`1`.
