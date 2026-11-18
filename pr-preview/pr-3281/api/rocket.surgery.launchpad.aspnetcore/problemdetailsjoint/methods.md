# ProblemDetailsJoint Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [ProblemDetailsJoint](/LaunchPad/pr-preview/pr-3281/api/rocket.surgery.launchpad.aspnetcore/problemdetailsjoint.md)
- Kind: `Methods`
- Members: `1`

Registers problem details. Rocket.Surgery.LaunchPad.AspNetCore.Mvc also fills in their title and type from MVC's client error mapping. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3281-merge.Sha.f470c0fad9a519f7357ef1c1f00f2210962f5308/src/AspNetCore/Joints/ProblemDetailsJoint.cs#L16)

Register the Service part.

```csharp
public class ProblemDetailsJoint
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
