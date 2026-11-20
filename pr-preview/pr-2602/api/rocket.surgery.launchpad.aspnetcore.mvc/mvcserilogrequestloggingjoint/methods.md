# MvcSerilogRequestLoggingJoint Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore.Mvc](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.aspnetcore.mvc.md)
- Type: [MvcSerilogRequestLoggingJoint](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.aspnetcore.mvc/mvcserilogrequestloggingjoint.md)
- Kind: `Methods`
- Members: `1`

Adds the MVC and Razor Pages filters that add the action and the page handler to the request's log event, in the diagnostic context `Joints.SerilogRequestLoggingJoint` registers. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2602-merge.Sha.022b9cee91d8927e916725e8a1c1717558619950/src/AspNetCore.Mvc/Joints/MvcSerilogRequestLoggingJoint.cs#L23-L29)

Register the Service part.

```csharp
public class MvcSerilogRequestLoggingJoint
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
- `services` (`IServiceCollection`)
