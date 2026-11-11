# SystemTextJsonCompositeNodaPatternConverter<T>

- Kind: `class`
- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.nodatime.md)
- Version: `2.0.1-pr.1167`
- Namespace: `Rocket.Surgery.LaunchPad.NodaTime`
- Target framework: `net10.0`
- Inherits: `NodaConverterBase<T>`

A JSON converter for types which can be represented by a single string value, parsed or formatted from an `Text.IPattern`1`.

## Definition

```csharp
namespace Rocket.Surgery.LaunchPad.NodaTime;

public sealed class SystemTextJsonCompositeNodaPatternConverter<T>
    : NodaTime.Serialization.SystemTextJson.NodaConverterBase<T>
{
    // ...
}
```

## Type Parameters

- `T` -- The type to convert to/from JSON.

## Remarks

Creates a new instance with a pattern and an optional validator. The validator will be called before each value is written, and may throw an exception to indicate that the value cannot be serialized.

## Constructors

- [SystemTextJsonCompositeNodaPatternConverter(Action<T>, IPattern<T>)](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.nodatime/systemtextjsoncompositenodapatternconverter-1/constructors.md#constructor-action-t-ipattern-t) -- A JSON converter for types which can be represented by a single string value, parsed or formatted from an `Text.IPattern`1`.
- [SystemTextJsonCompositeNodaPatternConverter(IPattern<T>)](/LaunchPad/pr-preview/pr-3175/api/rocket.surgery.launchpad.nodatime/systemtextjsoncompositenodapatternconverter-1/constructors.md#constructor-ipattern-t) -- Creates a new instance with a pattern and no validator.
