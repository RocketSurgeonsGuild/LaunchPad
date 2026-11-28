# PrimitiveError

- Kind: `struct`
- Package: [Rocket.Surgery.LaunchPad.Primitives](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives.md)
- Version: `2.0.1-pr.1304`
- Namespace: `Rocket.Surgery.LaunchPad.Primitives`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3332-merge.Sha.a12fd2e4649e7aaf51eae6effc83c67bf120ce1a/src/Primitives/PrimitiveError.cs)
- Implements: `IEquatable<PrimitiveError>`

Why a domain primitive rejected a value: a code ( `LENGTH`, `FORMAT`, `RANGE`, ...) and a message.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Primitives;

public sealed readonly struct PrimitiveError
    : System.IEquatable<Rocket.Surgery.LaunchPad.Primitives.PrimitiveError>
{
    // ...
}
```

## Constructors

- [PrimitiveError(string?, string?)](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/primitiveerror/constructors.md#constructor-string-string) -- Why a domain primitive rejected a value: a code ( `LENGTH`, `FORMAT`, `RANGE`, ...) and a message.

## Properties

- [Code](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/primitiveerror/properties.md#code) : `string?` `get; init` -- The code, or `null` when there is no error.
- [IsError](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/primitiveerror/properties.md#iserror) : `bool` `get` -- Whether this is an error.
- [Message](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/primitiveerror/properties.md#message) : `string?` `get; init` -- The message.
- [None](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/primitiveerror/properties.md#none) : [PrimitiveError](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/primitiveerror.md) `static` `get` -- No error.

## Methods

- [Create(string, string)](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/primitiveerror/methods.md#create-string-string) : [PrimitiveError](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/primitiveerror.md) `static` -- Creates an error.
- [Deconstruct(string?, string?)](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/primitiveerror/methods.md#deconstruct-string-string)
- [Equals(object)](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/primitiveerror/methods.md#equals-object) : `bool` -- Indicates whether this instance and a specified object are equal.
- [Equals(PrimitiveError)](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/primitiveerror/methods.md#equals-primitiveerror) : `bool` -- Indicates whether the current object is equal to another object of the same type.
- [GetHashCode](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/primitiveerror/methods.md#gethashcode) : `int` -- Returns the hash code for this instance.
- [op_Equality(PrimitiveError, PrimitiveError)](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/primitiveerror/methods.md#op-equality-primitiveerror-primitiveerror) : `bool` `static`
- [op_Inequality(PrimitiveError, PrimitiveError)](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/primitiveerror/methods.md#op-inequality-primitiveerror-primitiveerror) : `bool` `static`
- [ToString](/LaunchPad/pr-preview/pr-3332/api/rocket.surgery.launchpad.primitives/primitiveerror/methods.md#tostring) : `string` -- Returns the fully qualified type name of this instance.
