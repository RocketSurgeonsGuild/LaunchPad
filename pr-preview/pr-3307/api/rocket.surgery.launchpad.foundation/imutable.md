# IMutable

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1259`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3307-merge.Sha.0d6c4ca8b5b3c5055adee085d785c5712bbc3314/src/Foundation/IMutable.cs)

A mutable copy of an immutable value, such as the view model [MutableAttribute](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.foundation/mutableattribute.md) generates for a request, that can make the value it stands for.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public interface IMutable
{
    // ...
}
```

## Methods

- [ToImmutable](/LaunchPad/pr-preview/pr-3307/api/rocket.surgery.launchpad.foundation/imutable/methods.md#toimmutable) : `object` `abstract` -- Makes the immutable value from the current values of the mutable copy.
