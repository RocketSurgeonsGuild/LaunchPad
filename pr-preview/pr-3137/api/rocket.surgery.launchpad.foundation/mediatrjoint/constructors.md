# MediatRJoint Constructors

- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.foundation.md)
- Type: [MediatRJoint](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.foundation/mediatrjoint.md)
- Kind: `Constructors`
- Members: `1`

A joint that registers MediatR and every handler found in the assemblies that depend on it. Implements the `Joints.IServiceJoint`

<a id="constructor"></a>
<a id="constructor-foundationoptions"></a>

## MediatRJoint(FoundationOptions?)

- Name: `Constructor(FoundationOptions?)`

A joint that registers MediatR and every handler found in the assemblies that depend on it. Implements the `Joints.IServiceJoint`

```csharp
public class MediatRJoint
{
    public MediatRJoint(
        FoundationOptions? options = null)
    {
        // ...
    }
}
```

## Parameters

- `options` ([FoundationOptions?](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.foundation/foundationoptions.md)) `optional`
  The options, or `null` for the defaults.

## Remarks

The assemblies are found with the `ISpecularProvider` in the joint context. The generated `Imports` seed the application's Specular provider into the context.

Dovetail resolves the [FoundationOptions](/LaunchPad/pr-preview/pr-3137/api/rocket.surgery.launchpad.foundation/foundationoptions.md) from the joint context when it creates the joint, and they default when none were set.
