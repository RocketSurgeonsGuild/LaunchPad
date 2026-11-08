# IProblemDetailsData

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.320`
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

- [Instance](/LaunchPad/api/rocket.surgery.launchpad.primitives/iproblemdetailsdata/properties.md#instance) : `string?` `abstract` `get` -- The instance for the request
- [Link](/LaunchPad/api/rocket.surgery.launchpad.primitives/iproblemdetailsdata/properties.md#link) : `string?` `abstract` `get` -- Request Type
- [Properties](/LaunchPad/api/rocket.surgery.launchpad.primitives/iproblemdetailsdata/properties.md#properties) : `IDictionary<string, object?>` `abstract` `get` -- Additional properties
- [Title](/LaunchPad/api/rocket.surgery.launchpad.primitives/iproblemdetailsdata/properties.md#title) : `string?` `abstract` `get` -- Request title
