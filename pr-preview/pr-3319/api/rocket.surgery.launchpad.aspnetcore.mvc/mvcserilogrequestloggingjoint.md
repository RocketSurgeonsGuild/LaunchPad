# MvcSerilogRequestLoggingJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore.Mvc](/LaunchPad/pr-preview/pr-3319/api/rocket.surgery.launchpad.aspnetcore.mvc.md)
- Version: `2.0.1-pr.1271`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Mvc.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3319-merge.Sha.db5ef919c4f6a03a7f33e4672dc70757d169d5a4/src/AspNetCore.Mvc/Joints/MvcSerilogRequestLoggingJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

Adds the MVC and Razor Pages filters that add the action and the page handler to the request's log event, in the diagnostic context `Joints.SerilogRequestLoggingJoint` registers. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Mvc.Joints;

public class MvcSerilogRequestLoggingJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Constructors

- [MvcSerilogRequestLoggingJoint](/LaunchPad/pr-preview/pr-3319/api/rocket.surgery.launchpad.aspnetcore.mvc/mvcserilogrequestloggingjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3319/api/rocket.surgery.launchpad.aspnetcore.mvc/mvcserilogrequestloggingjoint/methods.md#register-idovetailcontext-iservicecollection) -- Register the Service part.

## See Also

- `Joints.IServiceJoint`
