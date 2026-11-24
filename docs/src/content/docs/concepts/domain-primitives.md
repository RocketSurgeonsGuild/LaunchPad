---
title: Domain primitives
description: A domain primitive holds a value of a domain concept and only accepts values its rules allow. Rocket.Surgery.LaunchPad.Primitives generates them from a declaration and one attribute.
---

A **domain primitive** is a type that holds a value of a domain concept (an email address, an amount, an order's ID) and only accepts
values its **rules** allow: there is no instance of it that breaks them. A domain primitive is one of these:

- a single value: a string, a number or a date, checked by its rules;
- a **strong ID**, which identifies an entity, so the IDs of different entities can't be mixed up;
- a **smart enum**, whose values are a fixed set of named members;
- a **value object**, which holds several values, validated together.

A **primitive kind** is a ready-made set of rules, such as email address, currency code or latitude, applied to a domain primitive
the application declares.

`Rocket.Surgery.LaunchPad.Primitives` depends on no other LaunchPad package, and nothing else brings it: an application or library opts in by
referencing it, and the [LaunchPad SDK](../launchpad-sdk/) never adds it.

```xml
<PackageReference Include="Rocket.Surgery.LaunchPad.Primitives" />
```

The package holds the interfaces the generated code implements and little else. Its analyzer package,
`Rocket.Surgery.LaunchPad.Primitives.Analyzers`, which it depends on, holds the generator, the analyzers and their code fixes. The
generator also writes the attributes into the project: they are `internal`, exist only in source (`[Conditional("CodeGeneration")]`),
and never reach the compiled assembly. A domain primitive in a referenced assembly is recognized by the interface the generator
implements: `IDomainPrimitive<TSelf, TValue>` for one value (a strong ID implements `IStrongId<TSelf, TValue>`, which extends it), and
`IValueObject<TSelf>` for a value object; both extend `IDomainPrimitive<TSelf>`.

The attributes, the interfaces and what you use day to day are in the `Rocket.Surgery.LaunchPad.Primitives` namespace. The rest is in
namespaces of its own: `.Advanced` (`PrimitiveBuilder<TPrimitive, TValue>`), `.Diagnostics` (`DomainPrimitivesInstrumentation`, the
meter and activity source), `.Descriptions` (what the generated `Description` of each domain primitive tells the integrations) and
`.Registries` (`Iso3166Countries` and `Iso4217Currencies`, the codes `[CountryCode]`, `[CurrencyCode]` and `[Money]` accept).

The reasons for this design are in
[ADR 0011](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/docs/adr/0011-primitives-is-a-port-of-dotnet-domain-primitives.md).

## Declaring a domain primitive

A domain primitive is a `readonly partial record struct` or a `sealed partial record` (a record class) with exactly one primitive
attribute, and any number of rules:

```csharp
using Rocket.Surgery.LaunchPad.Primitives;

[StringPrimitive, Trim, UpperCase, Length(3, 12), Regex("^[A-Z]{3}-[0-9]+$")]
public readonly partial record struct Sku;

[Email]
public sealed partial record CustomerEmail;

[StrongId<Guid>]
public readonly partial record struct OrderId;
```

The two forms behave the same, except for the missing value:

- A **struct** has a `default` that was never validated. `IsDefault` is `true` for it, and reading its `Value` throws. Prefer a
  struct: it doesn't allocate.
- A **class** has no default instance and no `IsDefault` of its own (through `IDomainPrimitive<TSelf>` it is `false`). A missing value
  is `null`, which nullable annotations track, and JSON `null` reads as a `null` reference.

The analyzers report a declaration that isn't `partial`, a struct that isn't `readonly`, a class that isn't `sealed` and a type that
isn't a record, and their code fixes add what is missing. The generator reports a type with more than one primitive attribute
(LPAD2102), and completes nothing for it.

### The kinds of domain primitives

| Attribute               | Value                                                                                  | What it is                                                                               |
| ----------------------- | -------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------- |
| `[StringPrimitive]`     | `string`                                                                               | A string checked by its rules.                                                           |
| `[NumericPrimitive<T>]` | any `INumber<T>`                                                                       | A number checked by its rules, with the arithmetic `Operations` allows.                  |
| `[DatePrimitive]`       | `DateOnly` (by default), `TimeOnly` or `DateTimeOffset`, or a NodaTime type, by `Kind` | A date or a time, optionally only in the past (`PastOnly`) or the future (`FutureOnly`). |
| `[StrongId<T>]`         | `Guid`, `int`, `long` or `string`                                                      | A strong ID.                                                                             |
| `[SmartEnum<T>]`        | the value of each member                                                               | A smart enum, whose members are its `public static readonly` fields of its own type.     |
| `[ValueObject]`         | its `init` properties                                                                  | A value object.                                                                          |
| a primitive kind        | set by the kind                                                                        | See [primitive kinds](#primitive-kinds).                                                 |

Every primitive attribute has `ErrorCode` and `ErrorMessage`, which replace the code and message of the errors its kind's own checks
report (the format of `[Email]`, the range of `[Percentage]`, the order of a `[DateRange]`). They don't replace the length checks a kind
brings (the maximum length of `[Email]`, say), the `EMPTY` of a strong ID, or the currency check of a pinned `[Money("USD")]`.

## Rules

Rules are attributes on the domain primitive. The normalizations change the value before it is checked; the checks reject it.

| Rule                            | Applies to        | What it does                                                                                                                           |
| ------------------------------- | ----------------- | -------------------------------------------------------------------------------------------------------------------------------------- |
| `[Trim]`                        | strings           | Removes white space from both ends.                                                                                                    |
| `[TrimStart]`, `[TrimEnd]`      | strings           | Removes white space from the start or the end.                                                                                         |
| `[LowerCase]`, `[UpperCase]`    | strings           | Changes the case (invariant culture). A type can't have both.                                                                          |
| `[NormalizeWhitespace]`         | strings           | Collapses every run of white space into one space.                                                                                     |
| `[Normalize<TNormalizer>]`      | any value         | Normalizes with an `INormalizer<T>`, after every built-in one. Repeatable.                                                             |
| `[NotEmpty]`                    | strings           | Rejects an empty or blank value.                                                                                                       |
| `[MinLength(n)]`                | strings           | At least `n` characters.                                                                                                               |
| `[MaxLength(n)]`                | strings           | At most `n` characters.                                                                                                                |
| `[Length(min, max)]`            | strings           | From `min` to `max` characters.                                                                                                        |
| `[ExactLength(n)]`              | strings           | Exactly `n` characters.                                                                                                                |
| `[Regex(pattern)]`              | strings           | Matches the pattern. Repeatable; the analyzers check the pattern.                                                                      |
| a `[GeneratedRegex]` member     | strings           | Matches the pattern, which the regular expression generator turns into code. See below.                                                |
| `[PrimitiveRange(min, max)]`    | numbers and dates | From `min` to `max`: numbers for a number, ISO 8601 text for a date or a time. `MinExclusive` and `MaxExclusive` leave out the bounds. |
| `[CustomValidator<TValidator>]` | any value         | Checks with an `ICustomValidator<T>`, after every other rule. Repeatable.                                                              |

The string rules apply to a string strong ID too, and `[PrimitiveRange]` to a strong ID of an `int` or a `long`. After the
built-in normalizations and before any `[Normalize<T>]`, a string is normalized to Unicode form C, and a string that can't be (an unpaired surrogate) is rejected with
`INVALID_UNICODE`. A string far longer than its maximum length is rejected before it is normalized, so a huge value costs nothing to
reject.

The pattern of a primitive kind is a `[GeneratedRegex]` of `PrimitivePatterns` (in `Rocket.Surgery.LaunchPad.Primitives.Advanced`), which
the regular expression generator turns into code when Primitives is built. A pattern of your own can be a `[GeneratedRegex]` too: every
static `[GeneratedRegex]` property, or method without parameters, of a string domain primitive or a string strong ID is a rule, checked
after its `[Regex]` rules, that reports `FORMAT` with the default message. Its description (and so the pattern of its OpenAPI schema)
is its pattern with the options that change what it matches written inline: `RegexOptions.IgnoreCase` makes `^[a-z]+$` `(?i)^[a-z]+$`.

```csharp
[StringPrimitive, Trim, UpperCase]
public readonly partial record struct Sku
{
    [GeneratedRegex("^[A-Z]{3}-[0-9]+$", RegexOptions.CultureInvariant, 1000)]
    private static partial Regex Format { get; }
}
```

You declare the `[GeneratedRegex]` yourself because the Primitives generator can't: the regular expression generator is a source generator
too, and one source generator doesn't see what another writes, so a `[GeneratedRegex]` the Primitives generator wrote would never be
implemented. A `[Regex]` is built once into a static `Regex` with `RegexOptions.CultureInvariant` and a one second timeout, and
interpreted rather than compiled, as native AOT interprets every pattern built at run time. LPAD2023 suggests a `[GeneratedRegex]` for a
`[Regex]` that doesn't set its own `ErrorCode` or `ErrorMessage` and comes after every `[Regex]` that does, and its fix writes it. The patterns of the kinds have the same one
second timeout. A match that times out rejects the value.

Every check but `[CustomValidator<T>]` has `ErrorCode` and `ErrorMessage` too; a custom validator returns its own `PrimitiveError`. A
normalizer and a custom validator are static, so nothing is allocated or resolved to run them:

```csharp
public sealed class NoTestSkus : ICustomValidator<string>
{
    public static PrimitiveError Validate(string value) => value.StartsWith("TST", StringComparison.Ordinal)
        ? PrimitiveError.Create("TEST_SKU", "Test SKUs are not allowed.")
        : PrimitiveError.None;
}

[StringPrimitive, Trim, UpperCase, ExactLength(8), CustomValidator<NoTestSkus>]
public readonly partial record struct Sku;
```

The project's settings add rules to every string domain primitive, string strong IDs and the string kinds included (see
[settings](#settings)): it is trimmed when `LaunchPadPrimitivesTrim` is `true`, must not be empty when `LaunchPadPrimitivesNotEmpty` is
`true` (a `[NotEmpty]` of its own keeps its error code and message), and is at most `LaunchPadPrimitivesMaxLength` characters (4096 by
default) when neither its rules nor its kind set a maximum length.

## Primitive kinds

A primitive kind is an attribute in place of `[StringPrimitive]`, `[NumericPrimitive<T>]` or `[DatePrimitive]`: it chooses the type of
the value and brings its rules. Its options are set per type.

### Text kinds

| Kind             | Rules                                                                                                                                      | Options                            |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------ | ---------------------------------- |
| `[Email]`        | An email address, trimmed and in lower case.                                                                                               | `MaxLength` (320)                  |
| `[Phone]`        | A phone number in E.164 format (`+14155552671`), trimmed.                                                                                  |                                    |
| `[Url]`          | An absolute URL with one of the allowed schemes, trimmed.                                                                                  | `AllowedSchemes` (`https`, `http`) |
| `[Slug]`         | Letters and digits separated by single hyphens, trimmed and lower case.                                                                    | `MaxLength` (200)                  |
| `[CountryCode]`  | An ISO 3166-1 alpha-2 country code, trimmed and upper case, that `Iso3166Countries` lists.                                                 |                                    |
| `[LanguageCode]` | Two letters, trimmed and lower case: the shape of an ISO 639-1 language code. The list of codes isn't checked.                             |                                    |
| `[CurrencyCode]` | An ISO 4217 currency code, trimmed and upper case, that `Iso4217Currencies` lists.                                                         |                                    |
| `[Username]`     | Letters, digits, dots, underscores and hyphens, trimmed.                                                                                   | `MinLength` (3), `MaxLength` (50)  |
| `[PasswordHash]` | Not empty.                                                                                                                                 |                                    |
| `[HexColor]`     | `#RGB`, `#RGBA`, `#RRGGBB` or `#RRGGBBAA`, trimmed and upper case.                                                                         |                                    |
| `[IPAddress]`    | An IPv4 or IPv6 address, trimmed.                                                                                                          |                                    |
| `[MacAddress]`   | Six pairs of hex digits separated by `:` or `-`, trimmed and upper case.                                                                   |                                    |
| `[IBAN]`         | An IBAN, trimmed and upper case, without spaces: a country code, two check digits and 11 to 30 letters and digits, which mod-97 validates. |                                    |
| `[ISBN]`         | The shape of an ISBN-10 or ISBN-13, trimmed, with or without hyphens or spaces. The check digit isn't verified.                            |                                    |
| `[VIN]`          | 17 letters and digits, without `I`, `O` and `Q`, trimmed and upper case. The check digit isn't verified.                                   |                                    |

### Number kinds

| Kind            | Value     | Rules                                                          | Options                            |
| --------------- | --------- | -------------------------------------------------------------- | ---------------------------------- |
| `[Percentage]`  | `decimal` | From `Min` to `Max`.                                           | `Min` (0), `Max` (100)             |
| `[Latitude]`    | `double`  | -90 to 90.                                                     |                                    |
| `[Longitude]`   | `double`  | -180 to 180.                                                   |                                    |
| `[Age]`         | `int`     | Years, from `Min` to `Max`.                                    | `Min` (0), `Max` (150)             |
| `[Weight]`      | `double`  | From `Min` to `Max`.                                           | `Min` (0), `Max` (1000)            |
| `[Height]`      | `double`  | From `Min` to `Max`.                                           | `Min` (0), `Max` (300)             |
| `[Distance]`    | `double`  | From `Min` to `Max`.                                           | `Min` (0), `Max` (no limit)        |
| `[Temperature]` | `double`  | From `Min` to `Max`.                                           | `Min` (-273.15), `Max` (no limit)  |
| `[Score]`       | `int`     | From `Min` to `Max`.                                           | `Min` (0), `Max` (100)             |
| `[Quantity]`    | `int`     | From `Min` to `Max`.                                           | `Min` (0), `Max` (no limit)        |
| `[Price]`       | `decimal` | From `Min` to `Max`.                                           | `Min` (0), `Max` (no limit)        |
| `[TaxRate]`     | `decimal` | A percentage, from `Min` to `Max`.                             | `Min` (0), `Max` (100)             |
| `[Discount]`    | `decimal` | A percentage, from `Min` to `Max`.                             | `Min` (0), `Max` (100)             |
| `[Rating]`      | `decimal` | From `Min` to `Max`, with at most `Scale` decimals.            | `Min` (0), `Max` (5), `Scale` (1)  |
| `[Month]`       | `int`     | 1 to 12.                                                       |                                    |
| `[Quarter]`     | `int`     | 1 to 4.                                                        |                                    |
| `[Week]`        | `int`     | 1 to 53 (ISO 8601 weeks), or 1 to 54 (weeks from January 1st). | `IsoWeekNumbering` (`true`)        |
| `[FiscalYear]`  | `int`     | From `MinYear` to `MaxYear`.                                   | `MinYear` (1900), `MaxYear` (9999) |

A domain primitive of a `float`, a `double` or a `Half`, of a numeric kind or of `[NumericPrimitive<T>]`, also rejects NaN and the
infinities.

### Date kinds

| Kind               | Value                                 | Rules                                        | Options                 |
| ------------------ | ------------------------------------- | -------------------------------------------- | ----------------------- |
| `[BirthDate]`      | `DateOnly`, or NodaTime's `LocalDate` | In the past, and at most `MaxAge` years ago. | `MaxAge` (150), `Kind`  |
| `[ExpirationDate]` | `DateOnly`, or NodaTime's `LocalDate` | In the future.                               | `Kind`                  |
| `[BusinessDate]`   | `DateOnly`, or NodaTime's `LocalDate` | A weekday. Public holidays are not checked.  | `AllowWeekends`, `Kind` |

A `Kind` other than `DateOnly` or `LocalDate` on one of these is LPAD2103.

### Value object kinds

| Kind          | Members                                                  | Rules                                                                | Options                                                   |
| ------------- | -------------------------------------------------------- | -------------------------------------------------------------------- | --------------------------------------------------------- |
| `[Money]`     | `decimal Amount`, `string Currency`                      | The currency is an ISO 4217 code; the amount is from `Min` to `Max`. | `[Money("USD")]` accepts only that currency; `Min`, `Max` |
| `[DateRange]` | `Start` and `End`: `DateOnly`, or NodaTime's `LocalDate` | `Start` is no later than `End`.                                      | `Kind`                                                    |
| `[TimeRange]` | `Start` and `End`: `TimeOnly`, or NodaTime's `LocalTime` | `Start` is no later than `End`, so a range can't run past midnight.  | `Kind`                                                    |

The generator declares the members of these kinds itself. `[Money]` trims the currency and makes it upper case, and checks it against
`Iso4217Currencies` (`CURRENCY`); `Min` and `Max` set no limit by default. It adds and subtracts only amounts in the same currency,
multiplies and divides by a `decimal`, and compares (`<`, `<=`, `>`, `>=`) only amounts in the same currency: amounts in two currencies
throw `InvalidOperationException`. `[Money("USD")]` also gets `Create(amount)` and `TryCreate(amount, ...)`, which take the amount
alone. `DateRange` and `TimeRange` have `Contains` and `Overlaps` (both ends included); `DateRange` has `Days`, the number of days with
both ends included, and `TimeRange` has `Duration`.

## What the generator writes

Every domain primitive of one value (a string, a number, a date or a strong ID) gets:

- `Value`, the value; and for a struct, `IsDefault`.
- `PrimitiveName`, its name in errors and diagnostics, and `Description`, the constraints of its rules for the integrations.
- `Create(value)`, which normalizes and checks the value and throws `DomainPrimitiveValidationException` (or the exception
  `LaunchPadPrimitivesExceptionType` names) when the rules reject it, and `TryCreate(value, out result, out error)`, which returns
  `false` and the `PrimitiveError` (a code and a message) instead.
- `Parse` and `TryParse` for strings, spans and UTF-8 (`IParsable<TSelf>`, `ISpanParsable<TSelf>`, `IUtf8SpanParsable<TSelf>`), so
  route, query and form values bind to it.
- `ToString` and `TryFormat` (`IFormattable`, `ISpanFormattable`, `IUtf8SpanFormattable`).
- Explicit conversions to and from the value. There are no implicit conversions: a domain primitive is only made by its rules.
- Equality, and comparison (`IComparable<TSelf>` and the comparison operators) when the value is comparable.
- A `TypeConverter` and a System.Text.Json converter (see [JSON](#json)).
- A nested `Errors` class with a constant for each code it can report, named in Pascal case: `Errors.NullInput` is `"NULL_INPUT"`.

A smart enum and a value object get less of this; their sections say what.

```csharp
var email = CustomerEmail.Create(" Someone@Example.com "); // "someone@example.com"

if (!Sku.TryCreate(input, out var sku, out var error))
{
    // error.Code is Sku.Errors.Length, Sku.Errors.Format, ... and error.Message says why
}
```

The codes are these, as they apply, and any `ErrorCode` a rule or a primitive attribute sets:

| Code              | Reported by                                                                                                                          |
| ----------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| `NULL_INPUT`      | a `null` string                                                                                                                      |
| `INVALID_UNICODE` | a string that isn't valid Unicode                                                                                                    |
| `EMPTY`           | an empty or blank string (`[NotEmpty]`, a kind that requires a value), or the empty value of a strong ID                             |
| `LENGTH`          | the length rules                                                                                                                     |
| `FORMAT`          | `[Regex]`, a `[GeneratedRegex]` member, and the format of a string kind (its pattern, its URL schemes, its ISO list, the IBAN check) |
| `RANGE`           | `[PrimitiveRange]`, the bounds of a kind, a `float`, `double` or `Half` that isn't finite                                            |
| `SCALE`           | `[Rating]` with more decimals than its `Scale`                                                                                       |
| `TEMPORAL`        | `PastOnly`, `FutureOnly`, and the checks of the date kinds                                                                           |
| `UNKNOWN_MEMBER`  | a value or a name that isn't a member of a smart enum                                                                                |
| `CURRENCY`        | a `[Money]` currency that isn't an ISO 4217 code, or not the currency of `[Money("USD")]`                                            |
| `INVARIANT`       | nothing generated: it is the code for a value object's own `Validate` to use                                                         |

A `[DateRange]`, a `[TimeRange]` and a `[Money]` with `Min` or `Max` report `RANGE` too.

`Rocket.Surgery.LaunchPad.Primitives` also has `ToDomainPrimitiveList` and `ToDomainPrimitiveArray`, which create a domain primitive
from each value of a sequence, and `PrimitiveBuilder<TPrimitive, TValue>` (in `.Advanced`), which adds checks decided at run time.

### Numbers

`Operations` on `[NumericPrimitive<T>]` chooses the arithmetic operators, and every result is checked by the rules again:

| `NumericOperations`    | Operators                                                   |
| ---------------------- | ----------------------------------------------------------- |
| `Addition`             | `a + b`                                                     |
| `Subtraction`          | `a - b`                                                     |
| `ScalarMultiplication` | `a * n`, with `n` of the value's type (there is no `n * a`) |
| `ScalarDivision`       | `a / n`, with `n` of the value's type                       |
| `Negation`             | `-a`                                                        |
| `Additive`             | addition, subtraction and negation                          |
| `Multiplicative`       | scalar multiplication and division                          |
| `All`                  | every operator                                              |

The arithmetic is `checked`, so an integer or a `decimal` that overflows throws `OverflowException`, and a result the rules reject
throws like `Create`.

```csharp
[NumericPrimitive<decimal>(Operations = NumericOperations.Additive), PrimitiveRange(0, 1_000_000)]
public readonly partial record struct Balance;
```

### Dates

`PastOnly`, `FutureOnly`, `[BirthDate]` and `[ExpirationDate]` compare against "now", read in UTC (today is the UTC date): a value
equal to now is in the past, not the future. `Create` and `TryCreate` of such a domain primitive have overloads that take where now
comes from: a `TimeProvider` for `DateOnly`, `TimeOnly` and `DateTimeOffset`, and NodaTime's `IClock` for the NodaTime types. Without
one they use the system clock. The helpers of the date kinds (`GetAge()` of a `[BirthDate]`, `IsExpired()` and `DaysUntilExpiration()`
of an `[ExpirationDate]`) take an optional `TimeProvider` or `IClock` the same way, so a test can fix the time. `PastOnly` and
`FutureOnly` have no meaning for a `Duration`, `Period`, `Offset` or `AnnualDate`, and setting both rejects every value: LPAD2020
reports either.

### Strong IDs

A strong ID also implements `IStrongId<TSelf, TValue>`:

- `Create()` makes a new ID of a `Guid`. `GuidVersion` chooses how: 7 (the default) for `Guid.CreateVersion7()`, which orders by time,
  or 4 for `Guid.NewGuid()`. Only a `Guid` ID can be created; any other throws `NotSupportedException`. A `GuidVersion` other than 4 or
  7, or one set on a strong ID that isn't of a `Guid`, is LPAD2103.
- `Empty` is the empty ID: for a class, one that holds `Guid.Empty`, 0 or an empty string; for a struct, its `default`.
  `RejectEmpty` (true by default) makes `Create(value)` reject the empty value (`Guid.Empty`, 0, or an empty or blank string) with
  `EMPTY`.

```csharp
[StrongId<Guid>(GuidVersion = 4)]
public readonly partial record struct SessionId;

[StrongId<long>(RejectEmpty = false)]
public readonly partial record struct LegacyCustomerId;
```

### Smart enums

A smart enum's members are its `public static readonly` fields of its own type, each made with its value through the private
constructor the generator writes, `new(value)`. A member's name is the name of its field. The smart enum gets:

- `Value`, `Name`, and for a struct `IsDefault` (the `default` is none of the members).
- `All`, every member in declaration order.
- `FromValue` and `TryFromValue`, which find a member by its value, and `Create` and `TryCreate`, which do the same and report
  `UNKNOWN_MEMBER` (`Errors.UnknownMember`) when no member has the value. `FromName` and `TryFromName` find a member by its name,
  optionally ignoring case; `FromName` throws `UNKNOWN_MEMBER` too.
- `Match`, which takes one function per member and returns what the one for this member returns; `Map`, the same with functions that
  are given the member; and `Switch`, which takes one action per member and runs the one for this member. Each has a parameter per
  member, so adding a member breaks every call that doesn't handle it.
- `Parse` and `TryParse` (`IParsable<TSelf>`), which take a name in any case or a value, and `ToString` (`IFormattable`), which is the
  name.
- An explicit conversion to its value, but none from it: `FromValue` says what happens when no member has it.
- A `TypeConverter` and a System.Text.Json converter.

A smart enum implements only `IParsable<TSelf>` and `IFormattable` of the parsing and formatting interfaces; the span and UTF-8 ones
(`ISpanParsable<TSelf>`, `ISpanFormattable` and the rest) are for the domain primitives of one value.

```csharp
[SmartEnum<int>]
public readonly partial record struct OrderStatus
{
    public static readonly OrderStatus Pending = new(1);
    public static readonly OrderStatus Shipped = new(2);
}

var label = status.Match(whenPending: () => "Waiting", whenShipped: () => "On its way");
status.Switch(whenPending: () => Remind(order), whenShipped: () => Track(order));
```

### Value objects

A value object's values are its `init` properties, and it implements `IValueObject<TSelf>`. It gets `PrimitiveName`, a
`Description` (its values, for the integrations), an `Errors` class, `Create(...)` and `TryCreate(..., out result, out error)` with a
parameter for each value, and a hook for the checks that involve several of them: implement the `static partial void Validate(ref T
value, ref PrimitiveError error)` it declares, and set `error` to reject the value. It runs after the checks of the kind. A value
object has no parsing, no conversions and no `TypeConverter`; a member that is itself a domain primitive brings its own rules.

A struct value object's `IsDefault` is `Equals(default)`: it has no field of its own to track creation, because a partial struct with
fields in more than one part has no defined layout (CS0282). So a struct value object whose values are all their defaults is
`IsDefault`, and JSON writes it as `null`.

```csharp
[ValueObject]
public readonly partial record struct ShippingWindow
{
    public DateOnly From { get; init; }
    public DateOnly To { get; init; }

    static partial void Validate(ref ShippingWindow value, ref PrimitiveError error)
    {
        if (value.To < value.From) error = PrimitiveError.Create(Errors.Invariant, "The window ends before it starts.");
    }
}
```

A property with `set` instead of `init` is LPAD2008, and one that is an array or a mutable collection is LPAD2018: use an immutable
collection.

## JSON

The generator writes a System.Text.Json converter for each domain primitive, which never falls back to reflection
([ADR 0007](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/docs/adr/0007-no-json-reflection.md)). Reading checks the rules,
so a rejected value fails to deserialize.

| Domain primitive                                         | JSON                                                                                                                                        |
| -------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------- |
| one value                                                | the value: `"someone@example.com"`, `42`, `"2026-11-23"`                                                                                    |
| smart enum                                               | its member's name in camel case (`"shipped"`); read by name in any case, or by value                                                        |
| value object                                             | an object of its values, named by the options' `PropertyNamingPolicy`, in camel case when there is none: `{"amount":12.5,"currency":"USD"}` |
| a missing value: a `null` class, or a struct's `default` | `null`, both ways                                                                                                                           |

A domain primitive of one value and a smart enum can also be the key of a dictionary: the key is the value's text (`ToString`) or the
member's name in camel case, parsed back through the rules.

A domain primitive of a NodaTime type reads and writes its value through the JSON options it is given, so it uses the NodaTime
converters of `Rocket.Surgery.LaunchPad.NodaTime` (applied by its `NodaTimeSystemTextJsonJoint`, or `ConfigureNodaTimeForLaunchPad`).
Primitives doesn't depend on that package, and LPAD2021 warns when a NodaTime domain primitive is declared where it isn't referenced.

The System.Text.Json source generator doesn't see what other generators write, so a `JsonSerializerContext` in the project that
declares a domain primitive can't see its converter. Declare domain primitives in a library the application references: a context in
the application sees the converter in the library's metadata, and so does the [JSON context](../../guides/json-and-native-aot/) LaunchPad
writes for an executable.

## NodaTime

When the project references NodaTime, `DatePrimitiveKind` has the NodaTime types as well: `Instant`, `OffsetDateTime`,
`ZonedDateTime`, `LocalDate`, `LocalTime`, `LocalDateTime`, `Duration`, `Period`, `AnnualDate`, `YearMonth` and `Offset`. Without
NodaTime those members don't exist, so a project can't name a type it can't compile. The package's targets define
`LAUNCHPAD_PRIMITIVES_NODATIME` when NodaTime is among the project's references, and the attributes the generator writes keep those
members behind it. `[BirthDate]`, `[ExpirationDate]`, `[BusinessDate]` and `[DateRange]` take `Kind = DatePrimitiveKind.LocalDate`,
and `[TimeRange]` takes `LocalTime`.

```csharp
[DatePrimitive(Kind = DatePrimitiveKind.Instant, PastOnly = true)]
public readonly partial record struct ShippedAt;
```

## Settings

| MSBuild property                           | Default     | What it does                                                                                                                                                                                                                              |
| ------------------------------------------ | ----------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `LaunchPadPrimitivesTrim`                  | `false`     | Trims every string domain primitive.                                                                                                                                                                                                      |
| `LaunchPadPrimitivesNotEmpty`              | `false`     | Rejects an empty or blank value for every string domain primitive.                                                                                                                                                                        |
| `LaunchPadPrimitivesMaxLength`             | `4096`      | The longest a string domain primitive may be when neither its rules nor its kind set a maximum length.                                                                                                                                    |
| `LaunchPadPrimitivesExceptionType`         |             | The full name of the exception `Create` throws instead of `DomainPrimitiveValidationException`, a nested type written `Outer.Inner`; not a generic type. It needs a constructor the project can access that takes the message (LPAD2017). |
| `LaunchPadPrimitivesOpenApiRepresentation` | `Component` | `Component`: a schema named after the domain primitive. `Inline`: the value's schema in place.                                                                                                                                            |
| `LaunchPadPrimitivesGraphQLRepresentation` | `Value`     | `Value`: the scalar of the value (`ID` for a strong ID). `Named`: a scalar named after the domain primitive.                                                                                                                              |

```xml
<PropertyGroup>
  <LaunchPadPrimitivesTrim>true</LaunchPadPrimitivesTrim>
  <LaunchPadPrimitivesOpenApiRepresentation>Inline</LaunchPadPrimitivesOpenApiRepresentation>
</PropertyGroup>
```

## In OpenAPI, GraphQL, EF Core and Dapper

An application that references Primitives itself gets [Joints](../joints/) that describe its domain primitives, and those of its
references, to the libraries it uses. They are [templates](../templates/#the-primitives-templates): written into the application only
when it references the library, so Primitives depends on none of them. They read a registry, `LaunchPadPrimitives`, that the registry
generator fills in at build time with every domain primitive the application declares or references and its rules, so nothing reflects
over them. An application whose domain primitives reach it only through a reference gets none of these Joints until it references
Primitives too: in its OpenAPI document every domain primitive would be an empty schema, because each is written by its own JSON
converter, whose output ASP.NET Core can't see, and EF Core, HotChocolate and Dapper couldn't map one. So LaunchPad's analyzer stops the
build of such an application with LPAD2024 when it references one of those libraries.

| Library                      | What a domain primitive becomes                                                                                                                                                                                                                                                                               |
| ---------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Microsoft.AspNetCore.OpenApi | the schema of its value with the constraints of its rules (lengths, pattern, range, format, and the member names of a smart enum), in bodies and in Minimal API and MVC parameters                                                                                                                            |
| HotChocolate                 | a scalar: the `ID` scalar for a strong ID, otherwise the scalar of its value (a value with no HotChocolate scalar, such as a NodaTime type, is left to the application), or one named after the domain primitive (`LaunchPadPrimitivesGraphQLRepresentation`); a value object is an object type of its values |
| Entity Framework Core        | a column of its value, with its maximum length and decimal scale, in every `DbContext` the application declares or references; a value object (`[Money]`, `[DateRange]`, `[TimeRange]` among them), struct or class, is a complex property                                                                    |
| Dapper                       | a parameter and a column of its value                                                                                                                                                                                                                                                                         |

For EF Core the registry generator writes a value converter for each domain primitive of one value, and
`LaunchPadPrimitivesEntityFrameworkJoint` configures every `DbContext` the application declares or references: it replaces the context's
`IValueConverterSelector`, so a domain primitive is a property of its value wherever it appears, and adds conventions that map every
value object as a complex property and give a key that is a strong ID of a `Guid` a value generator, which calls its `Create()` when
the entity is added. A context that replaces `IValueConverterSelector` itself keeps its own selector; it calls
`ConfigureDomainPrimitives()`, which the EF Core template declares on `LaunchPadPrimitivesEntityFramework`, from its own
`ConfigureConventions` instead:

```csharp
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) =>
    configurationBuilder.ConfigureDomainPrimitives();
```

For Dapper, when the application references it, the registry generator writes a type handler for each domain primitive of one value it
can see and a module initializer that registers them, so there is nothing to call. Two assemblies that register a handler for the same
type register the same handler.

## Metrics and traces

The generated `Create` and `TryCreate` record three counters on the `Rocket.Surgery.LaunchPad.Primitives` meter, each with a
`primitive_type` tag (and `error_type` for a failure):

| Counter                               | Counts                             |
| ------------------------------------- | ---------------------------------- |
| `domain_primitive.creation`           | domain primitives created          |
| `domain_primitive.validation.success` | values a domain primitive accepted |
| `domain_primitive.validation.failure` | values a domain primitive rejected |

A rejected value also adds a `domain_primitive.validation.failure` event to the current activity when the
`Rocket.Surgery.LaunchPad.Primitives` activity source has a listener. Nothing is recorded until something listens. The meter and the
activity source are `DomainPrimitivesInstrumentation.Meter` and `.ActivitySource`, in
`Rocket.Surgery.LaunchPad.Primitives.Diagnostics`. When the application references OpenTelemetry the registry generator writes
`AddLaunchPadPrimitivesInstrumentation()` for `MeterProviderBuilder` and `TracerProviderBuilder`, and the
`LaunchPadPrimitivesInstrumentationJoint` template calls it, so the [OpenTelemetry](../integration-packages/#opentelemetry) LaunchPad
sets up includes domain primitives with no code.

## Analyzers

LPAD2001 to LPAD2024 are analyzers. LPAD2024 is LaunchPad's own analyzer, which every LaunchPad application runs, as Primitives' analyzers
don't run in an application that only gets Primitives through a library. LPAD2101 to LPAD2103 are reported by the generator itself: they
are declarations it has to refuse to complete as written, so it is the generator that finds them.

| ID       | Severity | Reports                                                                                                                                                                                                                                                                                 |
| -------- | -------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| LPAD2001 | Error    | A domain primitive that isn't `partial`, or a type that contains one and isn't `partial` (code fix).                                                                                                                                                                                    |
| LPAD2002 | Error    | A struct domain primitive that isn't `readonly` (code fix).                                                                                                                                                                                                                             |
| LPAD2003 | Error    | A domain primitive that isn't a record (code fix).                                                                                                                                                                                                                                      |
| LPAD2004 | Error    | An invalid `[Regex]` pattern.                                                                                                                                                                                                                                                           |
| LPAD2005 | Error    | Both `[LowerCase]` and `[UpperCase]` (code fix).                                                                                                                                                                                                                                        |
| LPAD2006 | Error    | A minimum greater than the maximum: in the rules, in the bounds or defaults of the kind, or above `LaunchPadPrimitivesMaxLength`.                                                                                                                                                       |
| LPAD2007 | Warning  | `default`, `new()` or `new T()` of a struct domain primitive, which bypasses its rules.                                                                                                                                                                                                 |
| LPAD2008 | Error    | A value object property with `set` instead of `init` (code fix, unless the property is set after the value object is created).                                                                                                                                                          |
| LPAD2009 | Warning  | A `[StringPrimitive]`, `[NumericPrimitive<T>]`, `[DatePrimitive]` or string strong ID without a rule that checks its value (code fix for a `[StringPrimitive]`: add `[NotEmpty]`). A `[StringPrimitive]` isn't reported when `LaunchPadPrimitivesNotEmpty` is `true`.                   |
| LPAD2010 | Warning  | A string compared using `==` or `!=` with a domain primitive whose value is a string (code fix).                                                                                                                                                                                        |
| LPAD2011 | Warning  | A domain primitive whose value is a string used where a `string` is expected (code fix).                                                                                                                                                                                                |
| LPAD2012 | Warning  | A public constructor, which bypasses the rules (code fix).                                                                                                                                                                                                                              |
| LPAD2013 | Info     | Two domain primitives, other than strong IDs, with the same kind and rules.                                                                                                                                                                                                             |
| LPAD2014 | Info     | A domain primitive with more public members than its kind should need.                                                                                                                                                                                                                  |
| LPAD2015 | Info     | A public member of a domain primitive without XML documentation (code fix).                                                                                                                                                                                                             |
| LPAD2016 | Info     | A factory method not named `Create`, `TryCreate`, `Parse` or `TryParse` (code fix).                                                                                                                                                                                                     |
| LPAD2017 | Error    | A `LaunchPadPrimitivesExceptionType` the project can't find, can't access or finds in two references, or that isn't an exception with an accessible constructor taking a message.                                                                                                       |
| LPAD2018 | Warning  | A value object property that is a mutable collection or an array.                                                                                                                                                                                                                       |
| LPAD2019 | Error    | A class domain primitive that isn't `sealed` (code fix).                                                                                                                                                                                                                                |
| LPAD2020 | Error    | `PastOnly` or `FutureOnly` on a `Duration`, `Period`, `Offset` or `AnnualDate`, or both on one domain primitive.                                                                                                                                                                        |
| LPAD2021 | Warning  | A NodaTime domain primitive in a project that doesn't reference `Rocket.Surgery.LaunchPad.NodaTime`.                                                                                                                                                                                    |
| LPAD2022 | Warning  | A rule that doesn't apply to the kind: a string rule on a number, a date or a `Guid` strong ID, `[PrimitiveRange]` on a string, a string strong ID or a `Guid` strong ID, a `[PrimitiveRange]` of numbers on a date or of text on a number, any rule on a smart enum or a value object. |
| LPAD2023 | Info     | A `[Regex]` without its own `ErrorCode` or `ErrorMessage`, which can be a `[GeneratedRegex]` member (code fix).                                                                                                                                                                         |
| LPAD2024 | Error    | An application (not a test project) that uses the domain primitives of its references with OpenAPI, HotChocolate, EF Core or Dapper but doesn't reference Primitives itself, so it gets none of the Primitives Joints.                                                                  |
| LPAD2101 | Error    | A value type the primitive attribute doesn't support (a strong ID of a `double`, say), or a date `Kind` whose type the project doesn't reference.                                                                                                                                       |
| LPAD2102 | Error    | More than one primitive attribute on a type.                                                                                                                                                                                                                                            |
| LPAD2103 | Error    | An invalid attribute argument: a `GuidVersion` other than 4 or 7, `GuidVersion` on a strong ID that isn't of a `Guid`, a `Kind` the attribute doesn't take, a domain primitive nested in a generic type, or a `[Money]`, `[DateRange]` or `[TimeRange]` named like one of its values.   |

## Credits

Primitives is a port of [dotnet-domain-primitives](https://github.com/ericksonlopezf/dotnet-domain-primitives) by Erickson Lopez
(EricksonLopez.DomainPrimitives), under the MIT license, which
[THIRD-PARTY-NOTICES.md](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/THIRD-PARTY-NOTICES.md) reproduces and both
Primitives packages carry.
