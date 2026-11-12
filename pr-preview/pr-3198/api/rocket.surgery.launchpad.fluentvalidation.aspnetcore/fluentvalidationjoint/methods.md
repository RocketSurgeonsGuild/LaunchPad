# FluentValidationJoint Methods

- Package: [Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore](/LaunchPad/pr-preview/pr-3198/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore.md)
- Type: [FluentValidationJoint](/LaunchPad/pr-preview/pr-3198/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationjoint.md)
- Kind: `Methods`
- Members: `1`

Wires FluentValidation into ASP.NET Core: the client side adapters, the validation exception filter, the problem details converter and the source generated metadata it writes with, and the [FluentValidationProblemDetails](/LaunchPad/pr-preview/pr-3198/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails.md) as the documented validation response of the restful api controllers. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`

Registers the specified context.

```csharp
public class FluentValidationJoint
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
