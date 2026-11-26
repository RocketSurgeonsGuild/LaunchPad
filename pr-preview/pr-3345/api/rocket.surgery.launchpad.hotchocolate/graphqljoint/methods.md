# GraphqlJoint Methods

- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.hotchocolate.md)
- Type: [GraphqlJoint](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.hotchocolate/graphqljoint.md)
- Kind: `Methods`
- Members: `1`

The graph ql Joint

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3345-merge.Sha.a9c0325e0c8c70bc9d0569ac4df1fe5d29ffa4ec/src/HotChocolate/Joints/GraphqlJoint.cs#L20-L23)

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
