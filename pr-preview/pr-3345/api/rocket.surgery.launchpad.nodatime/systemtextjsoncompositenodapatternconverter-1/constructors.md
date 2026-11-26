# SystemTextJsonCompositeNodaPatternConverter<T> Constructors

- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.nodatime.md)
- Type: [SystemTextJsonCompositeNodaPatternConverter<T>](/LaunchPad/pr-preview/pr-3345/api/rocket.surgery.launchpad.nodatime/systemtextjsoncompositenodapatternconverter-1.md)
- Kind: `Constructors`
- Members: `2`

A JSON converter for types which can be represented by a single string value, parsed or formatted from an `Text.IPattern`1`.

<a id="constructor"></a>
<a id="constructor-action-t-ipattern-t"></a>

## SystemTextJsonCompositeNodaPatternConverter(Action<T>, IPattern<T>)

- Name: `Constructor(Action<T>, IPattern<T>)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3345-merge.Sha.a9c0325e0c8c70bc9d0569ac4df1fe5d29ffa4ec/src/NodaTime/SystemTextJsonCompositeNodaPatternConverter.cs#L25-L78)

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

The first pattern formats; reading tries each pattern in turn. The value is also read and written as a property name, so the type can be a dictionary key. A value no pattern parses throws `Json.JsonException`.

<a id="constructor-ipattern-t"></a>

## SystemTextJsonCompositeNodaPatternConverter(IPattern<T>)

- Name: `Constructor(IPattern<T>)`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/Branch.pull-3345-merge.Sha.a9c0325e0c8c70bc9d0569ac4df1fe5d29ffa4ec/src/NodaTime/SystemTextJsonCompositeNodaPatternConverter.cs#L31)

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
