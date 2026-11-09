# FluentValidationJoint Methods

- Package: [Rocket.Surgery.LaunchPad.FluentValidation](/LaunchPad/pr-preview/pr-3157/api/rocket.surgery.launchpad.fluentvalidation.md)
- Type: [FluentValidationJoint](/LaunchPad/pr-preview/pr-3157/api/rocket.surgery.launchpad.fluentvalidation/fluentvalidationjoint.md)
- Kind: `Methods`
- Members: `1`

A joint that registers every FluentValidation validator found in the assemblies that depend on FluentValidation, the validators for options, and the MediatR validation pipeline behaviors. Implements the `Joints.IServiceJoint`

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
  The joint context.
- `services` (`IServiceCollection`)
  The service collection.

## Exceptions

- `KeyNotFoundException` -- No `ISpecularProvider` was set in `context`.
