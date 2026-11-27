# SerilogRequestLoggingJoint Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3346/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [SerilogRequestLoggingJoint](/LaunchPad/pr-preview/pr-3346/api/rocket.surgery.launchpad.aspnetcore/serilogrequestloggingjoint.md)
- Kind: `Methods`
- Members: `1`

Wires Serilog's request logging into ASP.NET Core: the `IDiagnosticContext` that `UseLaunchPadRequestLogging` collects the request's properties in. Rocket.Surgery.LaunchPad.AspNetCore.Mvc adds the MVC and Razor Pages filters that add the action and the page handler. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3346-merge.Sha.ddf4e8df3db056914fd776aa443a00dc7536dd57/src/AspNetCore/Joints/SerilogRequestLoggingJoint.cs#L34-L36)

Registers the specified context.

```csharp
public class SerilogRequestLoggingJoint
{
    public void Register(
        IDovetailContext context,
        IServiceCollection services)
    {
        // ...
    }
}
```

## Parameters

- `context` (`IDovetailContext`)
  The context.
- `services` (`IServiceCollection`)
  The service collection.
