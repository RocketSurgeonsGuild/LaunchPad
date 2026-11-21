# GraphqlJoint Methods

- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/api/rocket.surgery.launchpad.hotchocolate.md)
- Type: [GraphqlJoint](/LaunchPad/api/rocket.surgery.launchpad.hotchocolate/graphqljoint.md)
- Kind: `Methods`
- Members: `1`

The graph ql Joint

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.d5743b8445e2048c34f1a4b585f3eb4d862008a8/src/HotChocolate/Joints/GraphqlJoint.cs#L20-L23)

Register the Service part.

```csharp
public class GraphqlJoint
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
