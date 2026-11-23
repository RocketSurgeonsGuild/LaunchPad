# HotChocolateJoint Methods

- Package: [Rocket.Surgery.LaunchPad.HotChocolate](/LaunchPad/api/rocket.surgery.launchpad.hotchocolate.md)
- Type: [HotChocolateJoint](/LaunchPad/api/rocket.surgery.launchpad.hotchocolate/hotchocolatejoint.md)
- Kind: `Methods`
- Members: `1`

Hot Chocolate Joint

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.main.Sha.54628ef221bd1ad09c6ebc55da16ddcb5027f4c7/src/HotChocolate/Joints/HotChocolateJoint.cs#L21-L26)

Register the Service part.

```csharp
public class HotChocolateJoint
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
