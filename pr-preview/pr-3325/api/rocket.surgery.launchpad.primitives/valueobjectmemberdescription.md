# ValueObjectMemberDescription

- Kind: `record`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1304`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives.Descriptions`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)
- Implements: `IEquatable<ValueObjectMemberDescription>`

One value a value object holds.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives.Descriptions;

public sealed record ValueObjectMemberDescription
    : System.IEquatable<Rocket.Surgery.LaunchPad.Primitives.Descriptions.ValueObjectMemberDescription>
{
    // ...
}
```

## Constructors

- [ValueObjectMemberDescription(string, Type, bool)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/constructors.md#constructor-string-type-bool) -- One value a value object holds.

## Properties

- [IsNullable](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/properties.md#isnullable) : `bool` `get; init` -- Whether the value may be missing (a nullable reference or value type).
- [Name](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/properties.md#name) : `string` `get; init` -- The name of its property.
- [Type](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/properties.md#type) : `Type` `get; init` -- The type of the value.

## Methods

- [<Clone>$](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/methods.md#clone) : [ValueObjectMemberDescription](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription.md)
- [Deconstruct(string, Type, bool)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/methods.md#deconstruct-string-type-bool)
- [Equals(object?)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [Equals(ValueObjectMemberDescription?)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/methods.md#equals-valueobjectmemberdescription) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [GetHashCode](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(ValueObjectMemberDescription?, ValueObjectMemberDescription?)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/methods.md#op-equality-valueobjectmemberdescription-valueobjectmemberdescription) : `bool` `static`
- [op_Inequality(ValueObjectMemberDescription?, ValueObjectMemberDescription?)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/methods.md#op-inequality-valueobjectmemberdescription-valueobjectmemberdescription) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription/methods.md#tostring) : `string` -- Returns a string that represents the current object.
