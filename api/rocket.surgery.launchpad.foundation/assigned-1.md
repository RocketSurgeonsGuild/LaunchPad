# Assigned<T>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.Foundation](/LaunchPad/api/rocket.surgery.launchpad.foundation.md)
- Version: `2.0.1-beta.326`
- Namespace: `Rocket.Surgery.LaunchPad.Foundation`
- Target framework: `net10.0`
- Implements: `IEquatable<Assigned<T>>`

The Assigned type is used to differentiate between not set and set input values.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.Foundation;

public class Assigned<T>
    : System.IEquatable<Rocket.Surgery.LaunchPad.Foundation.Assigned<T>>
{
    // ...
}
```

## Constructors

- [Assigned(T?)](/LaunchPad/api/rocket.surgery.launchpad.foundation/assigned-1/constructors.md#constructor-t) -- Initializes a new instance of the `Foundation.Assigned`1` struct.

## Properties

- [HasValue](/LaunchPad/api/rocket.surgery.launchpad.foundation/assigned-1/properties.md#hasvalue) : `bool` `get` -- `true` if the Assigned was explicitly set.
- [Value](/LaunchPad/api/rocket.surgery.launchpad.foundation/assigned-1/properties.md#value) : `T?` `get` -- The name value.

## Methods

- [Empty(T?)](/LaunchPad/api/rocket.surgery.launchpad.foundation/assigned-1/methods.md#empty-t) : [Assigned<T>](/LaunchPad/api/rocket.surgery.launchpad.foundation/assigned-1.md) `static` -- Creates an empty Assigned that provides a default value.
- [Equals(object?)](/LaunchPad/api/rocket.surgery.launchpad.foundation/assigned-1/methods.md#equals-object) : `bool` -- Compares this `Foundation.Assigned`1` value to another value.
- [Equals(Assigned<T>)](/LaunchPad/api/rocket.surgery.launchpad.foundation/assigned-1/methods.md#equals-assigned-t) : `bool` -- Compares this `Foundation.Assigned`1` value to another value.
- [GetHashCode](/LaunchPad/api/rocket.surgery.launchpad.foundation/assigned-1/methods.md#gethashcode) : `int` -- Serves as a hash function for a `Foundation.Assigned`1` object.
- [op_Equality(Assigned<T>, Assigned<T>)](/LaunchPad/api/rocket.surgery.launchpad.foundation/assigned-1/methods.md#op-equality-assigned-t-assigned-t) : `bool` `static` -- Operator call through to Equals
- [op_Implicit(T)](/LaunchPad/api/rocket.surgery.launchpad.foundation/assigned-1/methods.md#op-implicit-t) : [Assigned<T>](/LaunchPad/api/rocket.surgery.launchpad.foundation/assigned-1.md) `static` -- Implicitly creates a new `Foundation.Assigned`1` from the given value.
- [op_Implicit(Assigned<T>)](/LaunchPad/api/rocket.surgery.launchpad.foundation/assigned-1/methods.md#op-implicit-assigned-t) : `T?` `static` -- Implicitly gets the Assigned value.
- [op_Inequality(Assigned<T>, Assigned<T>)](/LaunchPad/api/rocket.surgery.launchpad.foundation/assigned-1/methods.md#op-inequality-assigned-t-assigned-t) : `bool` `static` -- Operator call through to Equals
- [ToString](/LaunchPad/api/rocket.surgery.launchpad.foundation/assigned-1/methods.md#tostring) : `string` -- Provides the name string.
