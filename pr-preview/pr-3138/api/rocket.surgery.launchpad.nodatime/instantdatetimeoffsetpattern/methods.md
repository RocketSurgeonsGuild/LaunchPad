# InstantDateTimeOffsetPattern Methods

- Package: [Rocket.Surgery.LaunchPad.NodaTime](/LaunchPad/pr-preview/pr-3138/api/rocket.surgery.launchpad.nodatime.md)
- Type: [InstantDateTimeOffsetPattern](/LaunchPad/pr-preview/pr-3138/api/rocket.surgery.launchpad.nodatime/instantdatetimeoffsetpattern.md)
- Kind: `Methods`
- Members: `3`

A pattern used to create an instant from a serialized DateTimeOffset value

<a id="appendformat"></a>
<a id="appendformat-instant-stringbuilder"></a>

## AppendFormat(Instant, StringBuilder)

- Name: `AppendFormat(Instant, StringBuilder)`
- Returns: `StringBuilder`

Formats the given value as text according to the rules of this pattern, appending to the given `Text.StringBuilder`.

```csharp
public class InstantDateTimeOffsetPattern
{
    public StringBuilder AppendFormat(
        Instant value,
        StringBuilder builder)
    {
        // ...
    }
}
```

## Parameters

- `value` (`Instant`)
  The value to format.
- `builder` (`StringBuilder`)
  The `StringBuilder` to append to.

## Returns

`StringBuilder` -- The builder passed in as `builder`.

<a id="format"></a>
<a id="format-instant"></a>

## Format(Instant)

- Name: `Format(Instant)`
- Returns: `string`

Formats the given value as text according to the rules of this pattern.

```csharp
public class InstantDateTimeOffsetPattern
{
    public string Format(
        Instant value)
    {
        // ...
    }
}
```

## Parameters

- `value` (`Instant`)
  The value to format.

## Returns

`string` -- The value formatted according to this pattern.

<a id="parse"></a>
<a id="parse-string"></a>

## Parse(string)

- Name: `Parse(string)`
- Returns: `ParseResult<Instant>`

Parses the given text value according to the rules of this pattern.

```csharp
public class InstantDateTimeOffsetPattern
{
    public ParseResult<Instant> Parse(
        string text)
    {
        // ...
    }
}
```

## Parameters

- `text` (`string`)
  The text value to parse.

## Returns

`ParseResult<Instant>` -- The result of parsing, which may be successful or unsuccessful.

## Remarks

This method never throws an exception (barring a bug in Noda Time itself). Even errors such as the argument being null are wrapped in a parse result.
