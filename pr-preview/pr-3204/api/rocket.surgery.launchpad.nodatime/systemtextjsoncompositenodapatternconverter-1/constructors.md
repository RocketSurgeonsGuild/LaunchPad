# SystemTextJsonCompositeNodaPatternConverter<T> Constructors

- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.nodatime.md)
- Type: [SystemTextJsonCompositeNodaPatternConverter<T>](/LaunchPad/pr-preview/pr-3204/api/rocket.surgery.launchpad.nodatime/systemtextjsoncompositenodapatternconverter-1.md)
- Kind: `Constructors`
- Members: `2`

A JSON converter for types which can be represented by a single string value, parsed or formatted from an `Text.IPattern`1`.

<a id="constructor"></a>
<a id="constructor-action-t-ipattern-t"></a>

## SystemTextJsonCompositeNodaPatternConverter(Action<T>, IPattern<T>)

- Name: `Constructor(Action<T>, IPattern<T>)`

A JSON converter for types which can be represented by a single string value, parsed or formatted from an `Text.IPattern`1`.

```csharp
public sealed class SystemTextJsonCompositeNodaPatternConverter<T>
{
    public SystemTextJsonCompositeNodaPatternConverter(
        Action<T>? validator,
        params IPattern<T>[] patterns)
    {
        // ...
    }
}
```

## Parameters

- `validator` (`Action<T>`)
  The validator to call before writing values. May be null, indicating that no validation is required.
- `patterns` (`IPattern<T>`)
  The patterns to use for parsing and formatting.

## Remarks

Creates a new instance with a pattern and an optional validator. The validator will be called before each value is written, and may throw an exception to indicate that the value cannot be serialized.

<a id="constructor-ipattern-t"></a>

## SystemTextJsonCompositeNodaPatternConverter(IPattern<T>)

- Name: `Constructor(IPattern<T>)`

Creates a new instance with a pattern and no validator.

```csharp
public sealed class SystemTextJsonCompositeNodaPatternConverter<T>
{
    public SystemTextJsonCompositeNodaPatternConverter(
        params IPattern<T>[] patterns)
    {
        // ...
    }
}
```

## Parameters

- `patterns` (`IPattern<T>`)
  The patterns to use for parsing and formatting.
