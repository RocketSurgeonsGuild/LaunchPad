# ZeroAllocHotChocolateAuthorizationJoint Methods

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Type: [ZeroAllocHotChocolateAuthorizationJoint](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/zeroallochotchocolateauthorizationjoint.md)
- Kind: `Methods`
- Members: `1`

Authorizes GraphQL fields through ASP.NET Core authorization, where the policies of a field's request are checked, states the scopes a field requires in the schema ( `@requiresScopes`, `@authenticated`), and reports a rejected field as the `Unauthorized` or `Forbidden` case is reported (docs/design/authorization.md). Implements the `Joints.IServiceJoint`

<a id="register"></a>
<a id="register-idovetailcontext-iservicecollection"></a>

## Register(IDovetailContext, IServiceCollection)

- Name: `Register(IDovetailContext, IServiceCollection)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.cda885391825d90598517a0aef31b3494df7c9f7/src/ZeroAlloc.HotChocolate/Joints/ZeroAllocHotChocolateAuthorizationJoint.cs#L22-L27)

Register the Service part.

```csharp
public class ZeroAllocHotChocolateAuthorizationJoint
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
