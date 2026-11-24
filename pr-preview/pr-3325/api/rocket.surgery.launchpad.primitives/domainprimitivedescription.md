# DomainPrimitiveDescription

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1304`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives.Descriptions`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3325-merge.Sha.f437060a172d9ddbe85bd230d99084b8367a7c51/src/Primitives/Descriptions/DomainPrimitiveDescription.cs)

What an integration needs to know about one domain primitive that holds one value: its value type, the format it is written in, and the constraints of its rules (lengths, patterns, bounds, allowed values). A rule written as code ( `[CustomValidator<T>]`) is not described.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives.Descriptions;

public abstract class DomainPrimitiveDescription
{
    // ...
}
```

## Remarks

The descriptions of an application's domain primitives are written by the Primitives generator, one closed generic per domain primitive, from the rules it read at compile time, so nothing here reflects over the types. [DomainPrimitiveDescription.Accept(IDomainPrimitiveVisitor<TResult>)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription/methods.md#accept-idomainprimitivevisitor-tresult) gives an integration the domain primitive and its value type as generic arguments.

## Properties

- [AllowedValues](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription/properties.md#allowedvalues) : `IReadOnlyList<string>` `get; init` -- The only values allowed, in ordinal order, when the rules list them (a smart enum's names).
- [Format](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription/properties.md#format) : `string?` `get; init` -- The format of the value (an OpenAPI / JSON Schema format such as `email` or `uri`), when its kind has one.
- [IsSmartEnum](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription/properties.md#issmartenum) : `bool` `get; init` -- Whether the domain primitive is a smart enum, whose [DomainPrimitiveDescription.AllowedValues](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription/properties.md#allowedvalues) are its names.
- [IsStrongId](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription/properties.md#isstrongid) : `bool` `get; init` -- Whether the domain primitive is a strong ID.
- [Maximum](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription/properties.md#maximum) : `string?` `get; init` -- The most a value may be, written in the invariant culture.
- [MaximumIsExclusive](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription/properties.md#maximumisexclusive) : `bool` `get; init` -- Whether [DomainPrimitiveDescription.Maximum](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription/properties.md#maximum) itself is excluded.
- [MaxLength](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription/properties.md#maxlength) : `int?` `get; init` -- The longest a text value may be.
- [Minimum](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription/properties.md#minimum) : `string?` `get; init` -- The least a value may be, written in the invariant culture.
- [MinimumIsExclusive](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription/properties.md#minimumisexclusive) : `bool` `get; init` -- Whether [DomainPrimitiveDescription.Minimum](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription/properties.md#minimum) itself is excluded.
- [MinLength](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription/properties.md#minlength) : `int?` `get; init` -- The shortest a text value may be.
- [Patterns](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription/properties.md#patterns) : `IReadOnlyList<string>` `get; init` -- The regular expressions a text value must match, every one of them.
- [Scale](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription/properties.md#scale) : `int?` `get; init` -- The number of digits after the decimal point a decimal value keeps, when its kind sets one.
- [Type](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription/properties.md#type) : `Type` `abstract` `get` -- The domain primitive.
- [ValueType](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription/properties.md#valuetype) : `Type` `abstract` `get` -- The value the domain primitive holds, which is what it is written as.

## Methods

- [Accept(IDomainPrimitiveVisitor<TResult>)](/LaunchPad/pr-preview/pr-3325/api/rocket.surgery.launchpad.primitives/domainprimitivedescription/methods.md#accept-idomainprimitivevisitor-tresult) : `TResult` `abstract` -- Hands the domain primitive and its value to `visitor` as generic arguments.
