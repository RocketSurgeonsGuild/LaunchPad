# MvcJoint Constructors

- Package: [Rocket.Surgery.LaunchPad.AspNetCore.Mvc](/LaunchPad/pr-preview/pr-3296/api/rocket.surgery.launchpad.aspnetcore.mvc.md)
- Type: [MvcJoint](/LaunchPad/pr-preview/pr-3296/api/rocket.surgery.launchpad.aspnetcore.mvc/mvcjoint.md)
- Kind: `Constructors`
- Members: `1`

Registers MVC Core, the MVC API explorer and the application parts for every assembly that depends on Rocket.Surgery.LaunchPad.AspNetCore. MVC is not native AOT compatible, which is why this joint lives in Rocket.Surgery.LaunchPad.AspNetCore.Mvc. Implements the `Joints.IServiceJoint`

<a id="constructor"></a>
<a id="constructor-aspnetcoremvcoptions"></a>

## MvcJoint(AspNetCoreMvcOptions?)

- Name: `Constructor(AspNetCoreMvcOptions?)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3296-merge.Sha.e2170d7d4b36cc435a264495ab512cf086f53980/src/AspNetCore.Mvc/Joints/MvcJoint.cs#L29-L111)

Registers MVC Core, the MVC API explorer and the application parts for every assembly that depends on Rocket.Surgery.LaunchPad.AspNetCore. MVC is not native AOT compatible, which is why this joint lives in Rocket.Surgery.LaunchPad.AspNetCore.Mvc. Implements the `Joints.IServiceJoint`

```csharp
public class MvcJoint
{
    public MvcJoint(
        AspNetCoreMvcOptions? options = null)
    {
        // ...
    }
}
```

## Parameters

- `options` ([AspNetCoreMvcOptions?](/LaunchPad/pr-preview/pr-3296/api/rocket.surgery.launchpad.aspnetcore.mvc/aspnetcoremvcoptions.md)) `optional`
  The options, or `null` for the defaults.

## Remarks

The application parts are found with the `ISpecularProvider` in the joint context. The generated `Imports` seed the application's Specular provider into the context.

Dovetail resolves the [AspNetCoreMvcOptions](/LaunchPad/pr-preview/pr-3296/api/rocket.surgery.launchpad.aspnetcore.mvc/aspnetcoremvcoptions.md) from the joint context when it creates the joint, and they default when none were set.
