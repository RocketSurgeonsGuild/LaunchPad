# IOptionalTracking<T>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3301/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1253`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3301-merge.Sha.477a5d9575a97343218a11eba2a3d0088808ddab/src/HotChocolate/IOptionalTracking.cs)

Marker interface used to create a record or class that copies any setable properties for classes and init/setable record properties

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.HotChocolate;

public interface IOptionalTracking<T>
    where T : IPropertyTracking
{
    // ...
}
```

## Remarks

This supports properties that are `Foundation.Assigned`1`, converting them to `Optional`1`
