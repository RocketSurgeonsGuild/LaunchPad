# IOptionalTracking<T>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3328/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-pr.1278`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3328-merge.Sha.e3c937ccb9d716ab226fa523eee028ac17c7ed68/src/HotChocolate/IOptionalTracking.cs)

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
