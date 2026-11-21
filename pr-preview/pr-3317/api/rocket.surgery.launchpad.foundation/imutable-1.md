# IMutable<T>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1271`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3317-merge.Sha.125c9fda2ca8a5e1aa1ca7ddbbdcadbcdb967166/src/Foundation/IMutable.cs)
- Implements: [IMutable](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.foundation/imutable.md)

A mutable copy of an immutable `T`, such as the view model [MutableAttribute](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.foundation/mutableattribute.md) generates for a request: a form binds to it a property at a time, and it makes the `T` to send.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public interface IMutable<T>
    : Rocket.Surgery.LaunchPad.Foundation.IMutable
{
    // ...
}
```

## Type Parameters

- `T` -- The immutable type.

## Methods

- [ToImmutable](/LaunchPad/pr-preview/pr-3317/api/rocket.surgery.launchpad.foundation/imutable-1/methods.md#toimmutable) : `T` `abstract` -- Makes the `T` from the current values of the mutable copy.
