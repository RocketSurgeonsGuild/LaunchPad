# StrongTypeDescription

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-beta.335`
- Namespace: `Rocket.Surgery.LaunchPad.StrongTypes`
- Target framework: `net10.0`

What an integration needs to know about one strong type: its value type, the format it is written in, and the constraints its rules set (lengths, patterns, bounds, allowed values), read from its SuperStrong validators. A rule written as a predicate ( `Satisfies`) is not described.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.StrongTypes;

public abstract class StrongTypeDescription
{
    // ...
}
```

## Remarks

The descriptions of an application's strong types are written by Primitives' source generator, one closed generic per strong type, so nothing here reflects over the types. [StrongTypeDescription.Accept(IStrongTypeVisitor<TResult>)](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription/methods.md#accept-istrongtypevisitor-tresult) gives an integration the strong type and its value type as generic arguments.

## Properties

- [AllowedValues](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription/properties.md#allowedvalues) : `IReadOnlyList<string>` `get` -- The only values allowed, written in the invariant culture, when the rules list them.
- [Format](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription/properties.md#format) : `string?` `get` -- The format of the value (an OpenAPI / JSON Schema format such as `email` or `uri`), when the strong type or its rules declare one with [StrongTypeFormatAttribute](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypeformatattribute.md).
- [Maximum](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription/properties.md#maximum) : `string?` `get` -- The most a number may be, written in the invariant culture.
- [MaximumIsExclusive](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription/properties.md#maximumisexclusive) : `bool` `get` -- Whether [StrongTypeDescription.Maximum](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription/properties.md#maximum) itself is excluded.
- [MaxLength](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription/properties.md#maxlength) : `int?` `get` -- The longest a text value may be.
- [Minimum](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription/properties.md#minimum) : `string?` `get` -- The least a number may be, written in the invariant culture.
- [MinimumIsExclusive](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription/properties.md#minimumisexclusive) : `bool` `get` -- Whether [StrongTypeDescription.Minimum](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription/properties.md#minimum) itself is excluded.
- [MinLength](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription/properties.md#minlength) : `int?` `get` -- The shortest a text value may be.
- [Patterns](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription/properties.md#patterns) : `IReadOnlyList<string>` `get` -- The regular expressions a text value must match, every one of them.
- [PrimitiveType](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription/properties.md#primitivetype) : `Type` `abstract` `get` -- The value the strong type wraps, which is what it is written as.
- [Type](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription/properties.md#type) : `Type` `abstract` `get` -- The strong type.

## Methods

- [Accept(IStrongTypeVisitor<TResult>)](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription/methods.md#accept-istrongtypevisitor-tresult) : `TResult` `abstract` -- Hands the strong type and its value to `visitor` as generic arguments.
- [Create(string?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription/methods.md#create-string) : [StrongTypeDescription](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription.md) `static` -- Describes a strong type whose value is not a number.
- [CreateNumber(string?)](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription/methods.md#createnumber-string) : [StrongTypeDescription](/LaunchPad/api/rocket.surgery.launchpad.primitives/strongtypedescription.md) `static` -- Describes a strong type whose value is a number, which also has bounds.
