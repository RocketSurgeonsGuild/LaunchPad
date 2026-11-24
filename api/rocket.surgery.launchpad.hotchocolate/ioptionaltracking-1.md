# IOptionalTracking<T>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/api/rocket.surgery.launchpad.hotchocolate.md)
- Version: `2.0.1-beta.451`
- Namespace: `Rocket.Surgery.LaunchPad.HotChocolate`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.4a5cb286621c756019f759ab46381031aa177050/src/HotChocolate/IOptionalTracking.cs)

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
