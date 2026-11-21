# ZeroAllocHotChocolateAuthorizationJoint Constructors

- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.HotChocolate](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate.md)
- Type: [ZeroAllocHotChocolateAuthorizationJoint](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.zeroalloc.hotchocolate/zeroallochotchocolateauthorizationjoint.md)
- Kind: `Constructors`
- Members: `1`

Authorizes GraphQL fields through ASP.NET Core authorization, where the policies of a field's request are checked, states the scopes a field requires in the schema ( `@requiresScopes`, `@authenticated`), and reports a rejected field as the `Unauthorized` or `Forbidden` case is reported (docs/design/authorization.md). Implements the `Joints.IServiceJoint`

<a id="constructor"></a>

## ZeroAllocHotChocolateAuthorizationJoint

- Name: `Constructor`

```csharp
public class ZeroAllocHotChocolateAuthorizationJoint
{
    public ZeroAllocHotChocolateAuthorizationJoint()
    {
        // ...
    }
}
```
