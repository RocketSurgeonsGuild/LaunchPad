# SerilogRequestLoggingJoint Constructors

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [SerilogRequestLoggingJoint](/LaunchPad/pr-preview/pr-3178/api/rocket.surgery.launchpad.aspnetcore/serilogrequestloggingjoint.md)
- Kind: `Constructors`
- Members: `1`

Wires Serilog's request logging into ASP.NET Core: the MVC and Razor Pages filters that add the action and the page handler to the request's log event, and the `IDiagnosticContext` they and `UseLaunchPadRequestLogging` collect the properties in. Implements the `Joints.IServiceJoint`

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
