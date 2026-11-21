# IMutable

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3312/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1275`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3312-merge.Sha.f73ef9701b863c2f6314f4b9416fd99efc8c6135/src/Foundation/IMutable.cs)

A mutable copy of an immutable value, such as the view model [MutableAttribute](/LaunchPad/pr-preview/pr-3312/api/rocket.surgery.launchpad.foundation/mutableattribute.md) generates for a request, that can make the value it stands for.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public interface IMutable
{
    // ...
}
```

## Methods

- [ToImmutable](/LaunchPad/pr-preview/pr-3312/api/rocket.surgery.launchpad.foundation/imutable/methods.md#toimmutable) : `object` `abstract` -- Makes the immutable value from the current values of the mutable copy.
