# FluentValidationJoint Constructors

- Package: [Rocket.Surgery.LaunchPad.FluentValidation](/LaunchPad/pr-preview/pr-3165/api/rocket.surgery.launchpad.fluentvalidation.md)
- Type: [FluentValidationJoint](/LaunchPad/pr-preview/pr-3165/api/rocket.surgery.launchpad.fluentvalidation/fluentvalidationjoint.md)
- Kind: `Constructors`
- Members: `1`

A joint that registers every FluentValidation validator found in the assemblies that depend on FluentValidation, the validators for options, and the MediatR validation pipeline behaviors. Implements the `Joints.IServiceJoint`

<a id="constructor"></a>
<a id="constructor-foundationoptions"></a>

## FluentValidationJoint(FoundationOptions?)

- Name: `Constructor(FoundationOptions?)`

A joint that registers every FluentValidation validator found in the assemblies that depend on FluentValidation, the validators for options, and the MediatR validation pipeline behaviors. Implements the `Joints.IServiceJoint`

```csharp
public class FluentValidationJoint
{
    public FluentValidationJoint(
        FoundationOptions? options = null)
    {
        // ...
    }
}
```

## Parameters

- `options` (`FoundationOptions?`) `optional`
  The options, or `null` for the defaults.

## Remarks

Runs after the `Joints.MediatRJoint`, so the validation behaviors are added to the pipeline MediatR registers.

The validators are found with the `ISpecularProvider` in the joint context. The generated `Imports` seed the application's Specular provider into the context.

Dovetail resolves the `Foundation.FoundationOptions` from the joint context when it creates the joint, and they default when none were set.
