# GraphqlJoint Methods

- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.hotchocolate.md)
- Type: [GraphqlJoint](/LaunchPad/pr-preview/pr-3308/api/rocket.surgery.launchpad.hotchocolate/graphqljoint.md)
- Kind: `Methods`
- Members: `1`

The graph ql Joint

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3308-merge.Sha.b5321c7138bef72baa9f203b3f824fa7207c9dcf/src/HotChocolate/Joints/GraphqlJoint.cs#L20-L23)

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
