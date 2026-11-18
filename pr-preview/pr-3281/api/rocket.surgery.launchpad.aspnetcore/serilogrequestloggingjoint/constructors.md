# SerilogRequestLoggingJoint Constructors

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [SerilogRequestLoggingJoint](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.aspnetcore/serilogrequestloggingjoint.md)
- Kind: `Constructors`
- Members: `1`

Wires Serilog's request logging into ASP.NET Core: the `IDiagnosticContext` that `UseLaunchPadRequestLogging` collects the request's properties in. Rocket.Surgery.LaunchPad.AspNetCore.Mvc adds the MVC and Razor Pages filters that add the action and the page handler. Implements the `Joints.IServiceJoint`

<a id="constructor"></a>

## SerilogRequestLoggingJoint

- Name: `Constructor`

```csharp
public class SerilogRequestLoggingJoint
{
    public SerilogRequestLoggingJoint()
    {
        // ...
    }
}
```
