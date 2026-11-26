# HealthChecksJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-pr.1290`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3345-merge.Sha.a9c0325e0c8c70bc9d0569ac4df1fe5d29ffa4ec/src/AspNetCore/Joints/HealthChecksJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

Registers the health check services and the options of LaunchPad's health check endpoints ( `MapLaunchPadHealthChecks`), bound from the [LaunchPadHealthCheckOptions.SectionName](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions/fields.md#sectionname) configuration section. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

public class HealthChecksJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

When [LaunchPadHealthCheckOptions.Port](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.aspnetcore/launchpadhealthcheckoptions/properties.md#port) is set, Kestrel also listens on that port, next to the application's own addresses, and the port serves the health check endpoints alone: anything else on it gets a 404, and so do the health check endpoints on any other port. When the server doesn't listen on the port after all, the health check endpoints answer on the application's own addresses.

## Constructors

- [HealthChecksJoint](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.aspnetcore/healthchecksjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.aspnetcore/healthchecksjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
