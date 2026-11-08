# MediatRJoint Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.foundation.md)
- Type: [MediatRJoint](/LaunchPad/pr-preview/pr-3026/api/rocket.surgery.launchpad.foundation/mediatrjoint.md)
- Kind: `Methods`
- Members: `1`

A joint that registers MediatR and every handler found in the assemblies that depend on it. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`

Registers the specified context.

```csharp
public class MediatRJoint
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
- `ArgumentException` -- No assemblies that reference MediatR were found.
