# SerilogRequestLoggingJoint

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore.md)
- Version: `2.0.1-beta.423`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Joints`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.bc6e38a474db052319ef3b044454738f3a433de2/src/AspNetCore/Joints/SerilogRequestLoggingJoint.cs)
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

- [SerilogRequestLoggingJoint](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/serilogrequestloggingjoint/constructors.md#constructor)

## Methods

- [Register(IDovetailContext, IServiceCollection)](/LaunchPad/api/rocket.surgery.launchpad.aspnetcore/serilogrequestloggingjoint/methods.md#register-idovetailcontext-iservicecollection) -- Registers the specified context.

## See Also

- `Joints.IServiceJoint`
