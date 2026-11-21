# MvcSerilogRequestLoggingJoint Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore.Mvc](/LaunchPad/pr-preview/pr-3312/api/rocket.surgery.launchpad.aspnetcore.mvc.md)
- Type: [MvcSerilogRequestLoggingJoint](/LaunchPad/pr-preview/pr-3312/api/rocket.surgery.launchpad.aspnetcore.mvc/mvcserilogrequestloggingjoint.md)
- Kind: `Methods`
- Members: `1`

Adds the MVC and Razor Pages filters that add the action and the page handler to the request's log event, in the diagnostic context `Joints.SerilogRequestLoggingJoint` registers. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3312-merge.Sha.f73ef9701b863c2f6314f4b9416fd99efc8c6135/src/AspNetCore.Mvc/Joints/MvcSerilogRequestLoggingJoint.cs#L23-L29)

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
