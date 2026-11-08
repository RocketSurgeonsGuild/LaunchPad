# AspNetCoreJoint Constructors

- Package: [Rocket.Surgery.LaunchPad.AspNetCore](/LaunchPad/pr-preview/pr-3153/api/rocket.surgery.launchpad.aspnetcore.md)
- Type: [AspNetCoreJoint](/LaunchPad/pr-preview/pr-3153/api/rocket.surgery.launchpad.aspnetcore/aspnetcorejoint.md)
- Kind: `Constructors`
- Members: `1`

Registers MVC, the API explorer and the application parts for every assembly that depends on this one. The other ASP.NET Core joints run after this one. Implements the `Joints.IServiceJoint`

<a id="constructor"></a>
<a id="constructor-aspnetcoreoptions"></a>

## AspNetCoreJoint(AspNetCoreOptions?)

- Name: `Constructor(AspNetCoreOptions?)`

Registers MVC, the API explorer and the application parts for every assembly that depends on this one. The other ASP.NET Core joints run after this one. Implements the `Joints.IServiceJoint`

```csharp
public class AspNetCoreJoint
{
    public AspNetCoreJoint(
        AspNetCoreOptions? options = null)
    {
        // ...
    }
}
```

## Parameters

- `options` ([AspNetCoreOptions?](/LaunchPad/pr-preview/pr-3153/api/rocket.surgery.launchpad.aspnetcore/aspnetcoreoptions.md)) `optional`
  The options, or `null` for the defaults.

## Remarks

The application parts are found with the `ISpecularProvider` in the joint context. The generated `Imports` seed the application's Specular provider into the context.

Dovetail resolves the [AspNetCoreOptions](/LaunchPad/pr-preview/pr-3153/api/rocket.surgery.launchpad.aspnetcore/aspnetcoreoptions.md) from the joint context when it creates the joint, and they default when none were set.
