# IMutable

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1259`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2602-merge.Sha.f387c871fa01cb1c7bfc78f0117912dc30d931f2/src/Foundation/IMutable.cs)

A mutable copy of an immutable value, such as the view model [MutableAttribute](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.foundation/mutableattribute.md) generates for a request, that can make the value it stands for.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public interface IMutable
{
    // ...
}
```

## Methods

- [ToImmutable](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.foundation/imutable/methods.md#toimmutable) : `object` `abstract` -- Makes the immutable value from the current values of the mutable copy.
