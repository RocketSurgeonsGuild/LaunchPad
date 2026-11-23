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
and never reach the compiled assembly. A domain primitive in a referenced assembly is recognized by the
`IDomainPrimitive<TSelf, TValue>` interface it implements. Everything is in the `Rocket.Surgery.LaunchPad.Primitives` namespace.

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
- A **class** has no default instance and no `IsDefault`. A missing value is `null`, which nullable annotations track, and JSON `null`
  reads as a `null` reference.

The analyzers report a declaration that isn't `partial`, a struct that isn't `readonly`, a class that isn't `sealed` and a type that
isn't a record, and their code fixes add what is missing.

### The kinds of domain primitives

| Attribute               | Value                                                                                  | What it is                                                                               |
| ----------------------- | -------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------- |
| `[StringPrimitive]`     | `string`                                                                               | A string checked by its rules.                                                           |
| `[NumericPrimitive<T>]` | any `INumber<T>`                                                                       | A number checked by its rules, with the arithmetic `Operations` allows.                  |
| `[DatePrimitive]`       | `DateOnly` (by default), `TimeOnly` or `DateTimeOffset`, or a NodaTime type, by `Kind` | A date or a time, optionally only in the past (`PastOnly`) or the future (`FutureOnly`). |
| `[StrongId<T>]`         | `Guid`, `int`, `long` or `string`                                                      | A strong ID.                                                                             |
| `[SmartEnum<T>]`        | the value of each member                                                               | A smart enum, whose members are its `public static readonly` fields.                     |
| `[ValueObject]`         | its `init` properties                                                                  | A value object.                                                                          |
| a primitive kind        | set by the kind                                                                        | See [primitive kinds](#primitive-kinds).                                                 |

Every primitive attribute has `ErrorCode` and `ErrorMessage`, which replace the code and message of the errors its own checks report.

## Rules

Rules are attributes on the domain primitive. The normalizations change the value before it is checked; the checks reject it.

| Rule                            | Applies to        | What it does                                                                                                                           |
| ------------------------------- | ----------------- | -------------------------------------------------------------------------------------------------------------------------------------- |
| `[Trim]`                        | strings           | Removes white space from both ends.                                                                                                    |
| `[TrimStart]`, `[TrimEnd]`      | strings           | Removes white space from the start or the end.                                                                                         |
| `[LowerCase]`, `[UpperCase]`    | strings           | Changes the case (invariant culture). A type can't have both.                                                                          |
| `[NormalizeWhitespace]`         | strings           | Collapses every run of white space into one space.                                                                                     |
| `[Normalize<TNormalizer>]`      | any value         | Normalizes with an `INormalizer<T>`, after the built-in normalizations. Repeatable.                                                    |
| `[NotEmpty]`                    | strings           | Rejects an empty or blank value.                                                                                                       |
| `[MinLength(n)]`                | strings           | At least `n` characters.                                                                                                               |
| `[MaxLength(n)]`                | strings           | At most `n` characters.                                                                                                                |
| `[Length(min, max)]`            | strings           | From `min` to `max` characters.                                                                                                        |
| `[ExactLength(n)]`              | strings           | Exactly `n` characters.                                                                                                                |
| `[Regex(pattern)]`              | strings           | Matches the pattern, through a `[GeneratedRegex]`. Repeatable; the analyzers check the pattern.                                        |
| `[PrimitiveRange(min, max)]`    | numbers and dates | From `min` to `max`: numbers for a number, ISO 8601 text for a date or a time. `MinExclusive` and `MaxExclusive` leave out the bounds. |
| `[CustomValidator<TValidator>]` | any value         | Checks with an `ICustomValidator<T>`, after every other rule. Repeatable.                                                              |

Every check has `ErrorCode` and `ErrorMessage` too. A normalizer and a custom validator are static, so nothing is allocated or
resolved to run them:

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

The project's settings add rules to every string domain primitive that doesn't set its own (see [settings](#settings)): it is trimmed
when `LaunchPadPrimitivesTrim` is `true`, must not be empty when `LaunchPadPrimitivesNotEmpty` is `true`, and is at most
`LaunchPadPrimitivesMaxLength` characters (4096 by default) when its rules set no length.

## Primitive kinds

A primitive kind is an attribute in place of `[StringPrimitive]`, `[NumericPrimitive<T>]` or `[DatePrimitive]`: it chooses the type of
the value and brings its rules. Its options are set per type.

### Text

| Kind             | Rules                                                      | Options                            |
| ---------------- | ---------------------------------------------------------- | ---------------------------------- |
| `[Email]`        | An email address, trimmed and in lower case.               | `MaxLength` (320)                  |
| `[Phone]`        | A phone number in E.164 format (`+14155552671`).           |                                    |
| `[Url]`          | An absolute URL with one of the allowed schemes.           | `AllowedSchemes` (`https`, `http`) |
| `[Slug]`         | Lower case letters and digits separated by single hyphens. | `MaxLength` (200)                  |
| `[CountryCode]`  | An ISO 3166-1 alpha-2 country code, upper case.            |                                    |
| `[LanguageCode]` | An ISO 639-1 language code, lower case.                    |                                    |
| `[CurrencyCode]` | An ISO 4217 currency code, upper case.                     |                                    |
| `[Username]`     | Letters, digits, dots, underscores and hyphens.            | `MinLength` (3), `MaxLength` (50)  |
| `[PasswordHash]` | Not empty.                                                 |                                    |
| `[HexColor]`     | `#RGB`, `#RGBA`, `#RRGGBB` or `#RRGGBBAA`, upper case.     |                                    |
| `[IPAddress]`    | An IPv4 or IPv6 address.                                   |                                    |
| `[MacAddress]`   | A MAC address, upper case.                                 |                                    |
| `[IBAN]`         | An IBAN, upper case.                                       |                                    |
| `[ISBN]`         | An ISBN-10 or ISBN-13.                                     |                                    |
| `[VIN]`          | A vehicle identification number, upper case.               |                                    |

### Numbers

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

### Dates

| Kind               | Value                                 | Rules                                        | Options                 |
| ------------------ | ------------------------------------- | -------------------------------------------- | ----------------------- |
| `[BirthDate]`      | `DateOnly`, or NodaTime's `LocalDate` | In the past, and at most `MaxAge` years ago. | `MaxAge` (150), `Kind`  |
| `[ExpirationDate]` | `DateOnly`, or NodaTime's `LocalDate` | In the future.                               | `Kind`                  |
| `[BusinessDate]`   | `DateOnly`, or NodaTime's `LocalDate` | A weekday. Public holidays are not checked.  | `AllowWeekends`, `Kind` |

### Value objects

| Kind          | Members                                                  | Rules                                                                | Options                                                   |
| ------------- | -------------------------------------------------------- | -------------------------------------------------------------------- | --------------------------------------------------------- |
| `[Money]`     | `decimal Amount`, `string Currency`                      | The currency is an ISO 4217 code; the amount is from `Min` to `Max`. | `[Money("USD")]` accepts only that currency; `Min`, `Max` |
| `[DateRange]` | `Start` and `End`: `DateOnly`, or NodaTime's `LocalDate` | `Start` is no later than `End`.                                      | `Kind`                                                    |
| `[TimeRange]` | `Start` and `End`: `TimeOnly`, or NodaTime's `LocalTime` | `Start` is no later than `End`, so a range can't run past midnight.  | `Kind`                                                    |

`Money` adds and subtracts only amounts in the same currency. `DateRange` and `TimeRange` have `Contains`, `Overlaps` and `Duration`.

## What the generator writes

Every domain primitive of one value gets:

- `Value`, the value; and for a struct, `IsDefault`.
- `PrimitiveName`, its name in errors and diagnostics.
- `Create(value)`, which normalizes and checks the value and throws `DomainPrimitiveValidationException` (or the exception
  `LaunchPadPrimitivesExceptionType` names) when the rules reject it, and `TryCreate(value, out result, out error)`, which returns
  `false` and the `PrimitiveError` (a code and a message) instead.
- `Parse` and `TryParse` for strings, spans and UTF-8 (`IParsable<TSelf>`, `ISpanParsable<TSelf>`, `IUtf8SpanParsable<TSelf>`), so
  route, query and form values bind to it.
- `ToString` and `TryFormat` (`IFormattable`, `ISpanFormattable`, `IUtf8SpanFormattable`).
- Explicit conversions to and from the value. There are no implicit conversions: a domain primitive is only made by its rules.
- Equality, and comparison (`IComparable<TSelf>` and the comparison operators) when the value is comparable.
- A `TypeConverter` and a System.Text.Json converter (see [JSON](#json)).
- An `Errors` class with the codes it reports as constants (`NULL_INPUT`, `EMPTY`, `LENGTH`, `FORMAT`, `RANGE`, `TEMPORAL`,
  `INVARIANT`, as they apply).

```csharp
var email = CustomerEmail.Create(" Someone@Example.com "); // "someone@example.com"

if (!Sku.TryCreate(input, out var sku, out var error))
{
    // error.Code is "LENGTH", "FORMAT", ... and error.Message says why
}
```

`Rocket.Surgery.LaunchPad.Primitives` also has `ToDomainPrimitiveList` and `ToDomainPrimitiveArray`, which create a domain primitive
from each value of a sequence, and `PrimitiveBuilder<TPrimitive, TValue>` (in `.Advanced`), which adds checks decided at run time.

### Numbers

`Operations` on `[NumericPrimitive<T>]` chooses the arithmetic operators, and every result is checked by the rules again:

| `NumericOperations`    | Operators                                  |
| ---------------------- | ------------------------------------------ |
| `Addition`             | `a + b`                                    |
| `Subtraction`          | `a - b`                                    |
| `ScalarMultiplication` | `a * n` and `n * a`, with `n` of the value |
| `ScalarDivision`       | `a / n`                                    |
| `Negation`             | `-a`                                       |
| `Additive`             | addition, subtraction and negation         |
| `Multiplicative`       | scalar multiplication and division         |
| `All`                  | every operator                             |

```csharp
[NumericPrimitive<decimal>(Operations = NumericOperations.Additive), PrimitiveRange(0, 1_000_000)]
public readonly partial record struct Balance;
```

### Dates

`PastOnly`, `FutureOnly` and the date kinds compare against "now". `Create` and `TryCreate` have overloads that take where now comes
from: a `TimeProvider` for `DateOnly`, `TimeOnly` and `DateTimeOffset`, and NodaTime's `IClock` for the NodaTime types. Without one they
use the system clock. The helpers of the date kinds (the age of a `[BirthDate]`, whether an `[ExpirationDate]` has passed and the days
until it does) take a `TimeProvider` or an `IClock` the same way, so a test can fix the time. `PastOnly` and `FutureOnly` have no
meaning for a `Duration`, `Period`, `Offset` or `AnnualDate`, and an analyzer reports them there.

### Strong IDs

A strong ID also implements `IStrongId<TSelf, TValue>`:

- `Create()` makes a new ID of a `Guid`. `GuidVersion` chooses how: 7 (the default) for `Guid.CreateVersion7()`, which orders by time,
  or 4 for `Guid.NewGuid()`. Only a `Guid` ID can be created; any other throws `NotSupportedException`.
- `Empty` is the empty ID (`Guid.Empty`, 0 or an empty string). `RejectEmpty` (true by default) makes `Create(value)` reject it.

```csharp
[StrongId<Guid>(GuidVersion = 4)]
public readonly partial record struct SessionId;

[StrongId<long>(RejectEmpty = false)]
public readonly partial record struct LegacyCustomerId;
```

### Smart enums

A smart enum's members are its `public static readonly` fields, each made with its value and its name through a constructor the
generator writes. It gets `Name`, every member in `All`, `FromValue` and `TryFromValue`, which
find a member by its value, `FromName` and `TryFromName`, which find it by its name (optionally ignoring case), and `Match`, which
takes one function per member and calls the one for the member it is, so adding a member breaks every `Match` that doesn't handle it.

```csharp
[SmartEnum<int>]
public readonly partial record struct OrderStatus
{
    public static readonly OrderStatus Pending = new(1, "Pending");
    public static readonly OrderStatus Shipped = new(2, "Shipped");
}

var label = status.Match(whenPending: () => "Waiting", whenShipped: () => "On its way");
```

### Value objects

A value object's values are its `init` properties. It gets `Create(...)` and `TryCreate(..., out result, out error)` with a parameter
for each, and a hook for the checks that involve several of them: implement the `static partial void Validate(ref T value, ref
PrimitiveError error)` it declares, and set `error` to reject the value.

```csharp
[ValueObject]
public readonly partial record struct ShippingWindow
{
    public DateOnly From { get; init; }
    public DateOnly To { get; init; }

    static partial void Validate(ref ShippingWindow value, ref PrimitiveError error)
    {
        if (value.To < value.From) error = PrimitiveError.Create("INVARIANT", "The window ends before it starts.");
    }
}
```

Its properties should not be mutable collections or arrays; an analyzer suggests an immutable or read-only collection.

## JSON

The generator writes a System.Text.Json converter for each domain primitive, which never falls back to reflection
([ADR 0007](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/docs/adr/0007-no-json-reflection.md)). Reading checks the rules,
so a rejected value fails to deserialize.

| Domain primitive      | JSON                                                                         |
| --------------------- | ---------------------------------------------------------------------------- |
| one value             | the value: `"someone@example.com"`, `42`, `"2026-11-23"`                     |
| smart enum            | its member's name in camel case (`"shipped"`); read by name or by value      |
| value object          | an object with camel case property names: `{"amount":12.5,"currency":"USD"}` |
| a class with no value | `null`                                                                       |

A domain primitive of a NodaTime type reads and writes its value through the JSON options it is given, so it uses the NodaTime
converters of `Rocket.Surgery.LaunchPad.NodaTime` (applied by its `NodaTimeSystemTextJsonJoint`, or `ConfigureNodaTimeForLaunchPad`).
Primitives doesn't depend on that package, and an analyzer warns when a NodaTime domain primitive is declared where it isn't referenced.

The System.Text.Json source generator doesn't see what other generators write, so a `JsonSerializerContext` in the project that
declares a domain primitive can't see its converter. Declare domain primitives in a library the application references: a context in
the application sees the converter in the library's metadata, and so does the [JSON context](../../guides/json-and-native-aot/) LaunchPad
writes for an executable.

## NodaTime

When the project references NodaTime, `DatePrimitiveKind` has the NodaTime types as well: `Instant`, `OffsetDateTime`,
`ZonedDateTime`, `LocalDate`, `LocalTime`, `LocalDateTime`, `Duration`, `Period`, `AnnualDate`, `YearMonth` and `Offset`. Without
NodaTime those members don't exist, so a project can't name a type it can't compile. `[BirthDate]`, `[ExpirationDate]`,
`[BusinessDate]` and `[DateRange]` take `Kind = DatePrimitiveKind.LocalDate`, and `[TimeRange]` takes `LocalTime`.

```csharp
[DatePrimitive(Kind = DatePrimitiveKind.Instant, PastOnly = true)]
public readonly partial record struct ShippedAt;
```

## Settings

| MSBuild property                           | Default     | What it does                                                                                                                                          |
| ------------------------------------------ | ----------- | ----------------------------------------------------------------------------------------------------------------------------------------------------- |
| `LaunchPadPrimitivesTrim`                  | `false`     | Trims every string domain primitive.                                                                                                                  |
| `LaunchPadPrimitivesNotEmpty`              | `false`     | Rejects an empty or blank value for every string domain primitive.                                                                                    |
| `LaunchPadPrimitivesMaxLength`             | `4096`      | The longest a string domain primitive may be when its rules set no length.                                                                            |
| `LaunchPadPrimitivesExceptionType`         |             | The full name of the exception `Create` throws instead of `DomainPrimitiveValidationException`. It needs a public constructor that takes the message. |
| `LaunchPadPrimitivesOpenApiRepresentation` | `Component` | `Component`: a schema named after the domain primitive. `Inline`: the value's schema in place.                                                        |
| `LaunchPadPrimitivesGraphQLRepresentation` | `Value`     | `Value`: the scalar of the value (`ID` for a strong ID). `Named`: a scalar named after the domain primitive.                                          |

```xml
<PropertyGroup>
  <LaunchPadPrimitivesTrim>true</LaunchPadPrimitivesTrim>
  <LaunchPadPrimitivesOpenApiRepresentation>Inline</LaunchPadPrimitivesOpenApiRepresentation>
</PropertyGroup>
```

## In OpenAPI, GraphQL, EF Core and Dapper

An application that references Primitives itself gets [Joints](../joints/) that describe its domain primitives, and those of its
references, to the libraries it uses. They are [templates](../templates/#the-primitives-templates): written into the application only
when it references the library, so Primitives depends on none of them. They read a registry, `LaunchPadPrimitives`, that the generator
fills in at build time with every domain primitive the application declares or references and its rules, so nothing reflects over them.

| Library                      | What a domain primitive becomes                                                                                                                                                                                  |
| ---------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Microsoft.AspNetCore.OpenApi | the schema of its value with the constraints of its rules (lengths, pattern, range, format, and the member names of a smart enum), in bodies and in Minimal API and MVC parameters                               |
| HotChocolate                 | a scalar: the `ID` scalar for a strong ID, otherwise the scalar of its value or one named after the domain primitive                                                                                             |
| Entity Framework Core        | a column of its value, with its maximum length and precision, in every `DbContext` the application declares or references; a value object (`[Money]`, `[DateRange]`, `[TimeRange]` among them) is a complex type |
| Dapper                       | a parameter and a column of its value                                                                                                                                                                            |

For EF Core the generator writes a `ValueConverter` for each domain primitive and `ConfigureDomainPrimitives(ModelConfigurationBuilder)`,
which `LaunchPadPrimitivesEntityFrameworkJoint` applies to every `DbContext`. A context can also call it from its own
`ConfigureConventions`. A key that is a strong ID of a `Guid` gets a value generator that calls its `Create()`.

For Dapper, when the application references it, the generator writes a type handler for each domain primitive it can see and a module
initializer that registers them, so there is nothing to call. Two assemblies that register a handler for the same type register the same
handler.

## Metrics and traces

The generated `Create` and `TryCreate` record three counters on the `Rocket.Surgery.LaunchPad.Primitives` meter, each with a
`primitive_type` tag (and `error_type` for a failure):

| Counter                               | Counts                             |
| ------------------------------------- | ---------------------------------- |
| `domain_primitive.creation`           | domain primitives created          |
| `domain_primitive.validation.success` | values a domain primitive accepted |
| `domain_primitive.validation.failure` | values a domain primitive rejected |

A rejected value also adds a `domain_primitive.validation.failure` event to the current activity when the
`Rocket.Surgery.LaunchPad.Primitives` activity source has a listener. Nothing is recorded until something listens. When the application
references OpenTelemetry the generator writes `AddLaunchPadPrimitivesInstrumentation()` for `MeterProviderBuilder` and
`TracerProviderBuilder`, and the `LaunchPadPrimitivesInstrumentationJoint` template calls it, so the
[OpenTelemetry](../integration-packages/#opentelemetry) LaunchPad sets up includes domain primitives with no code.

## Analyzers

| ID       | Severity | Reports                                                                                                           |
| -------- | -------- | ----------------------------------------------------------------------------------------------------------------- |
| LPAD2001 | Error    | A domain primitive that isn't `partial` (code fix).                                                               |
| LPAD2002 | Error    | A struct domain primitive that isn't `readonly` (code fix).                                                       |
| LPAD2003 | Error    | A domain primitive that isn't a record (code fix).                                                                |
| LPAD2004 | Error    | An invalid `[Regex]` pattern.                                                                                     |
| LPAD2005 | Error    | Both `[LowerCase]` and `[UpperCase]`.                                                                             |
| LPAD2006 | Error    | A minimum greater than the maximum.                                                                               |
| LPAD2007 | Warning  | `default` or `new()` of a struct domain primitive, which bypasses its rules.                                      |
| LPAD2008 | Error    | A value object property without `init`.                                                                           |
| LPAD2009 | Warning  | A domain primitive without rules (code fix).                                                                      |
| LPAD2010 | Warning  | A string compared with a domain primitive using `==` (code fix).                                                  |
| LPAD2011 | Warning  | A domain primitive assigned to a `string` (code fix).                                                             |
| LPAD2012 | Warning  | A public constructor, which bypasses the rules.                                                                   |
| LPAD2013 | Info     | Two domain primitives with the same rules.                                                                        |
| LPAD2014 | Info     | A domain primitive with more public members than recommended.                                                     |
| LPAD2015 | Info     | A public member of a domain primitive without XML documentation (code fix).                                       |
| LPAD2016 | Info     | A factory method not named `Create`, `TryCreate`, `Parse` or `TryParse` (code fix).                               |
| LPAD2017 | Error    | A `LaunchPadPrimitivesExceptionType` that isn't an exception with a public constructor taking a message.          |
| LPAD2018 | Warning  | A value object property that is a mutable collection or an array.                                                 |
| LPAD2019 | Error    | `PastOnly` or `FutureOnly` on a `Duration`, `Period`, `Offset` or `AnnualDate`.                                   |
| LPAD2020 | Error    | A class domain primitive that isn't `sealed` (code fix).                                                          |
| LPAD2021 | Error    | `GuidVersion` on a strong ID that isn't of a `Guid`.                                                              |
| LPAD2022 | Warning  | A NodaTime domain primitive in a project that doesn't reference `Rocket.Surgery.LaunchPad.NodaTime`.              |
| LPAD2101 | Error    | A value type the primitive attribute doesn't support (a strong ID of a `double`, say).                            |
| LPAD2102 | Error    | More than one primitive attribute on a type.                                                                      |
| LPAD2103 | Error    | An invalid attribute argument, such as a `GuidVersion` other than 4 or 7, or a `Kind` the attribute doesn't take. |

## Credits

Primitives is a port of [dotnet-domain-primitives](https://github.com/ericksonlopezf/dotnet-domain-primitives) by Erickson Lopez
(EricksonLopez.DomainPrimitives), under the MIT license, which
[THIRD-PARTY-NOTICES.md](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/THIRD-PARTY-NOTICES.md) reproduces and both
Primitives packages carry.
