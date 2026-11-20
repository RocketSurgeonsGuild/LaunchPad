# IPropertyTracking<T>

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3305/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1256`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3305-merge.Sha.f536a04ca4e28fbdc06da7c1e0f968bbeccd9e9a/src/Foundation/IPropertyTracking.cs)
- Implements: [IPropertyTracking](/LaunchPad/pr-preview/pr-3305/api/rocket.surgery.launchpad.foundation/ipropertytracking.md)

Marker interface used to create a record or class that copies any setable properties for classes and init/setable record properties

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public interface IPropertyTracking<T>
    : Rocket.Surgery.LaunchPad.Foundation.IPropertyTracking
{
    // ...
}
```

## Methods

- [ApplyChanges(T)](/LaunchPad/pr-preview/pr-3305/api/rocket.surgery.launchpad.foundation/ipropertytracking-1/methods.md#applychanges-t) : `T` `abstract` -- Method used apply changes from a given property tracking class.
- [ResetChanges](/LaunchPad/pr-preview/pr-3305/api/rocket.surgery.launchpad.foundation/ipropertytracking-1/methods.md#resetchanges) `abstract` -- Reset the state of the property tracking to only track future changes
