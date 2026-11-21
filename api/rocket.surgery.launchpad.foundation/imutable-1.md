# IMutable<T>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-beta.432`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.d5743b8445e2048c34f1a4b585f3eb4d862008a8/src/Foundation/IMutable.cs)
- Implements: [IMutable](/LaunchPad/api/rocket.surgery.launchpad.foundation/imutable.md)

A mutable copy of an immutable `T`, such as the view model [MutableAttribute](/LaunchPad/api/rocket.surgery.launchpad.foundation/mutableattribute.md) generates for a request: a form binds to it a property at a time, and it makes the `T` to send.

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

- [ToImmutable](/LaunchPad/api/rocket.surgery.launchpad.foundation/imutable-1/methods.md#toimmutable) : `T` `abstract` -- Makes the `T` from the current values of the mutable copy.
