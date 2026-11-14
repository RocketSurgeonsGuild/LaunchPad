# FluentValidationOpenApiJoint Methods

- Package: [Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore.md)
- Type: [FluentValidationOpenApiJoint](/LaunchPad/pr-preview/pr-3210/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationopenapijoint.md)
- Kind: `Methods`
- Members: `1`

Adds the experimental FluentValidation schema transformer to the OpenAPI document, so the validation rules show in the schemas. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`

Register the Service part.

```csharp
public class FluentValidationOpenApiJoint
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
