# IProblemDetailsData

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3157/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1158`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net10.0`

A common interface used for definiting problems

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives;

public interface IProblemDetailsData
{
    // ...
}
```

## Properties

- [Instance](/LaunchPad/pr-preview/pr-3157/api/rocket.surgery.launchpad.primitives/iproblemdetailsdata/properties.md#instance) : `string?` `abstract` `get` -- The instance for the request
- [Link](/LaunchPad/pr-preview/pr-3157/api/rocket.surgery.launchpad.primitives/iproblemdetailsdata/properties.md#link) : `string?` `abstract` `get` -- Request Type
- [Properties](/LaunchPad/pr-preview/pr-3157/api/rocket.surgery.launchpad.primitives/iproblemdetailsdata/properties.md#properties) : `IDictionary<string, object?>` `abstract` `get` -- Additional properties
- [Title](/LaunchPad/pr-preview/pr-3157/api/rocket.surgery.launchpad.primitives/iproblemdetailsdata/properties.md#title) : `string?` `abstract` `get` -- Request title
