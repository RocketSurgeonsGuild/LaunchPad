# DefaultJoint Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3293/api/rocket.surgery.launchpad.foundation.md)
- Type: [DefaultJoint](/LaunchPad/pr-preview/pr-3293/api/rocket.surgery.launchpad.foundation/defaultjoint.md)
- Kind: `Methods`
- Members: `1`

The joint every LaunchPad application starts from. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3293-merge.Sha.4c1c9433962eeb0eff759138d3819e6ad0cb6311/src/Foundation/Joints/DefaultJoint.cs#L32-L40)

Registers the specified context.

```csharp
public class DefaultJoint
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
