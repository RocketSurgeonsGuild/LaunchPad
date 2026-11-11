# ValidationFailureDetail

- Kind: `record`
- Package: [Rocket.Surgery.LaunchPad.ZeroAlloc.AspNetCore](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore.md)
- Version: `2.0.1-beta.332`
- Namespace: `Rocket.Surgery.LaunchPad.AspNetCore.Validation`
- Target framework: `net10.0`
- Implements: `IEquatable<ValidationFailureDetail>`

A single validation failure.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.AspNetCore.Validation;

public record ValidationFailureDetail
    : System.IEquatable<Rocket.Surgery.LaunchPad.AspNetCore.Validation.ValidationFailureDetail>
{
    // ...
}
```

## Constructors

- [ValidationFailureDetail(string, string, string?)](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail/constructors.md#constructor-string-string-string) -- A single validation failure.

## Properties

- [ErrorCode](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail/properties.md#errorcode) : `string?` `get; init` -- The code of the rule that failed.
- [ErrorMessage](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail/properties.md#errormessage) : `string` `get; init` -- Why it failed.
- [PropertyName](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail/properties.md#propertyname) : `string` `get; init` -- The property that failed.

## Methods

- [<Clone>$](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail/methods.md#clone) : [ValidationFailureDetail](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail.md) `virtual`
- [Deconstruct(string, string, string?)](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail/methods.md#deconstruct-string-string-string)
- [Equals(object?)](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail/methods.md#equals-object) : `bool` -- Determines whether the specified object is equal to the current object.
- [Equals(ValidationFailureDetail?)](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail/methods.md#equals-validationfailuredetail) : `bool` `virtual` -- Indicates whether the current object is equal to another object of the same type.
- [GetHashCode](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail/methods.md#gethashcode) : `int` -- Serves as the default hash function.
- [op_Equality(ValidationFailureDetail?, ValidationFailureDetail?)](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail/methods.md#op-equality-validationfailuredetail-validationfailuredetail) : `bool` `static`
- [op_Inequality(ValidationFailureDetail?, ValidationFailureDetail?)](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail/methods.md#op-inequality-validationfailuredetail-validationfailuredetail) : `bool` `static`
- [ToString](/LaunchPad/api/rocket.surgery.launchpad.zeroalloc.aspnetcore/validationfailuredetail/methods.md#tostring) : `string` -- Returns a string that represents the current object.
