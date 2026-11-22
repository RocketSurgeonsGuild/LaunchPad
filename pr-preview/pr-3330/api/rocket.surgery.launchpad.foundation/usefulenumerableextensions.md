# UsefulEnumerableExtensions

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3330/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1282`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation.Extensions`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3330-merge.Sha.8f1e0adb726291eaf19241f8ed2aef23d09d52f6/src/Foundation/Extensions/UsefulEnumerableExtensions.cs)

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

- [GetAsyncEnumerator(IAsyncEnumerator<T>)](/LaunchPad/pr-preview/pr-3330/api/rocket.surgery.launchpad.foundation/usefulenumerableextensions/methods.md#getasyncenumerator-iasyncenumerator-t) : `IAsyncEnumerator<T>` `extension` -- Allows you to enumerate over an enumerator you have without the need for it to be an `Generic.IAsyncEnumerator`1`.
- [GetEnumerator(IEnumerator<T>)](/LaunchPad/pr-preview/pr-3330/api/rocket.surgery.launchpad.foundation/usefulenumerableextensions/methods.md#getenumerator-ienumerator-t) : `IEnumerator<T>` `extension` -- Allows you to enumerate over an enumerator you have without the need for it to be an `Generic.IEnumerable`1`.
