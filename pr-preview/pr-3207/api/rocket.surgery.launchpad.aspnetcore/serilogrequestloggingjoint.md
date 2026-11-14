# SerilogRequestLoggingJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-pr.1222`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Joints`
- Target framework: `net10.0`
- Implements: `IDovetailJoint`, `IServiceJoint`

Wires Serilog's request logging into ASP.NET Core: the MVC and Razor Pages filters that add the action and the page handler to the request's log event, and the `IDiagnosticContext` they and `UseLaunchPadRequestLogging` collect the properties in. Implements the `Joints.IServiceJoint`

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

- [SerilogRequestLoggingJoint](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.aspnetcore/serilogrequestloggingjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/pr-preview/pr-3207/api/rocket.surgery.launchpad.aspnetcore/serilogrequestloggingjoint/methods.md#register-idovetailcontext-iservicecollection) -- Registers the specified context.

## See Also

- `Joints.IServiceJoint`
