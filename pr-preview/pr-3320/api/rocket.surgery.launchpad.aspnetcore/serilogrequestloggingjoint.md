# SerilogRequestLoggingJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-pr.1272`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3320-merge.Sha.bb93b2eb5e069135a2caef17855a85e8a18d22af/src/AspNetCore/Joints/SerilogRequestLoggingJoint.cs)
- Implements: `IDovetailJoint`, `IServiceJoint`

Wires Serilog's request logging into ASP.NET Core: the `IDiagnosticContext` that `UseLaunchPadRequestLogging` collects the request's properties in. Rocket.Surgery.LaunchPad.AspNetCore.Mvc adds the MVC and Razor Pages filters that add the action and the page handler. Implements the `Joints.IServiceJoint`

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Joints;

public class SerilogRequestLoggingJoint
    : Joinery.Dovetail.Infrastructure.IDovetailJoint,
      Joinery.Dovetail.Joints.IServiceJoint
{
    // ...
}
```

## Remarks

The diagnostic context is added with `TryAdd`, the same as the one LaunchPad.Hosting writes into an application that references Joinery.Dovetail.Serilog, so whichever registers first is used. Both bind the properties through the Serilog `Core.Logger` in the container; this one falls back to the static logger when there is none.

## Constructors

- [SerilogRequestLoggingJoint](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.aspnetcore/serilogrequestloggingjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3320/api/rocket.surgery.launchpad.aspnetcore/serilogrequestloggingjoint/methods.md#register-idovetailcontext-iservicecollection) -- Registers the specified context.

## See Also

- `Joints.IServiceJoint`
