# IProblemDetailsData

- Kind: `interface`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-pr.1261`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3308-merge.Sha.b5321c7138bef72baa9f203b3f824fa7207c9dcf/src/Foundation/IProblemDetailsData.cs)

A common interface used for definiting problems

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public interface IProblemDetailsData
{
    // ...
}
```

## Properties

- [Instance](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.foundation/iproblemdetailsdata/properties.md#instance) : `string?` `abstract` `get` -- The instance for the request
- [Link](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.foundation/iproblemdetailsdata/properties.md#link) : `string?` `abstract` `get` -- Request Type
- [Properties](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.foundation/iproblemdetailsdata/properties.md#properties) : `IDictionary<string, object?>` `abstract` `get` -- Additional properties
- [Title](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.foundation/iproblemdetailsdata/properties.md#title) : `string?` `abstract` `get` -- Request title
