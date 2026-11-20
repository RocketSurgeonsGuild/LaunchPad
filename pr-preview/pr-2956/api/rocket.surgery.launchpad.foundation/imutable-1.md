# IMutable<T>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-2956/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1260`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2956-merge.Sha.e1593c4f42603ac4a494d6f792fa4b45d268d971/src/Foundation/IMutable.cs)
- Implements: [IMutable](/LaunchPad/pr-preview/pr-2956/api/rocket.surgery.launchpad.foundation/imutable.md)

A mutable copy of an immutable `T`, such as the view model [MutableAttribute](/LaunchPad/pr-preview/pr-2956/api/rocket.surgery.launchpad.foundation/mutableattribute.md) generates for a request: a form binds to it a property at a time, and it makes the `T` to send.

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

- [ToImmutable](/LaunchPad/pr-preview/pr-2956/api/rocket.surgery.launchpad.foundation/imutable-1/methods.md#toimmutable) : `T` `abstract` -- Makes the `T` from the current values of the mutable copy.
