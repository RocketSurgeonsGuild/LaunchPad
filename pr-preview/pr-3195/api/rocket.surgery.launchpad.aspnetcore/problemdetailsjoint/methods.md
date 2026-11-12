# ProblemDetailsJoint Methods

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [ProblemDetailsJoint](/LaunchPad/pr-preview/pr-3195/api/rocket.surgery.launchpad.aspnetcore/problemdetailsjoint.md)
- Kind: `Methods`
- Members: `1`

Registers problem details and the exception handler status codes for the LaunchPad exceptions. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`

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
