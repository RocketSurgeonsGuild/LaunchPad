# MvcProblemDetailsJoint Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore.Mvc](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.aspnetcore.mvc.md)
- Type: [MvcProblemDetailsJoint](/LaunchPad/pr-preview/pr-2602/api/rocket.surgery.launchpad.aspnetcore.mvc/mvcproblemdetailsjoint.md)
- Kind: `Methods`
- Members: `1`

Fills in the title and type of problem details from the client error mapping of MVC's `Mvc.ApiBehaviorOptions`, and turns off MVC's automatic 400 for an invalid model state (LaunchPad validates requests itself). Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-2602-merge.Sha.f387c871fa01cb1c7bfc78f0117912dc30d931f2/src/AspNetCore.Mvc/Joints/MvcProblemDetailsJoint.cs#L25-L42)

Register the Service part.

```csharp
public class MvcProblemDetailsJoint
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
