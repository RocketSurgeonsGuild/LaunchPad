# DefaultJoint Methods

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.foundation.md)
- Type: [DefaultJoint](/LaunchPad/pr-preview/pr-3337/api/rocket.surgery.launchpad.foundation/defaultjoint.md)
- Kind: `Methods`
- Members: `1`

The joint every LaunchPad application starts from. Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3337-merge.Sha.b3468f8f7f271d2dc63ee1d3333c2bcdd48e93a2/src/Foundation/Joints/DefaultJoint.cs#L32-L40)

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
