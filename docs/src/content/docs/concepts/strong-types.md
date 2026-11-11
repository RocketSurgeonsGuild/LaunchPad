---
title: Strong types
description: A strong type wraps one value of a domain concept and is valid by construction. Rocket.Surgery.LaunchPad.Primitives ships ready-made strong types and the rules sets they are built on.
---

A **strong type** wraps a single value of a domain concept (an email address, a currency code, an amount) and is valid by
construction: there is no instance of it that breaks its rules. A **rules set** is a named, reusable set of the constraints a strong
type is valid under, so several strong types can share them.

`Rocket.Surgery.LaunchPad.Primitives` ships ready-made strong types built on [SuperStrong](https://superstrong.dev). It depends on
no other LaunchPad package, and nothing else brings it: an application or library opts in by referencing it. Its types live in the
`Rocket.Surgery.LaunchPad.StrongTypes` namespace.

```xml
<PackageReference Include="Rocket.Surgery.LaunchPad.Primitives" />
```

## Using a strong type

Every strong type has the surface SuperStrong generates:

- `From(value)` returns the strong type, or throws `StrongTypeValidationException` with every rule the value breaks.
- `TryFrom(value, out result)` returns `false` instead of throwing.
- `AsPrimitive()` returns the wrapped value.
- Value equality and comparison, `Parse` and `TryParse`, formatting, a `TypeConverter`, and a System.Text.Json converter that writes
  the wrapped value and checks the rules when it reads one.

```csharp
using Rocket.Surgery.LaunchPad.StrongTypes;

var email = EmailAddress.From("someone@example.com");

if (!CurrencyCode.TryFrom(input, out var currency))
{
    // not a current ISO 4217 code
}
```

SuperStrong checks a value without changing it, so nothing is normalized. Where a standard says a value is case-insensitive (country,
currency and language codes, hexadecimal digits), the rules accept only the standard's canonical case and reject the other: `US`, not
`us`.

## The ready-made types

Each type is a sealed class built on the rules set of the same name with `Rules` after it (`EmailAddress` on `EmailAddressRules`).
The XML documentation of each rules set gives the full rules and the standard behind them.

### Text

| Type           | Value    | Rules                                                                                                                                                                                                                                      |
| -------------- | -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `EmailAddress` | `string` | An RFC 5322 `addr-spec` RFC 5321 delivers to: at most 254 characters, a `dot-atom` local part of at most 64, and a domain of at least two host name labels. No quoted local parts, address literals or non-ASCII characters. Case is kept. |
| `PhoneNumber`  | `string` | ITU-T E.164: `+`, then 7 to 15 digits, the first not 0. No spaces or punctuation.                                                                                                                                                          |
| `Url`          | `string` | An absolute `http` or `https` URI (RFC 3986) with a host, the scheme in lower case, only characters RFC 3986 allows (others percent-encoded), at most 8000 characters.                                                                     |
| `Slug`         | `string` | Lower-case ASCII letters and digits in groups separated by single hyphens, at most 100 characters.                                                                                                                                         |
| `Username`     | `string` | 3 to 32 ASCII letters and digits, with single `.`, `_` or `-` between them. Case is kept.                                                                                                                                                  |
| `HexColor`     | `string` | CSS `#rgb` or `#rrggbb` in lower case. No alpha channel.                                                                                                                                                                                   |
| `PasswordHash` | `string` | Not empty or white space; the format belongs to the hashing function. `ToString()` writes `***`.                                                                                                                                           |

### Codes and identifiers

| Type           | Value    | Rules                                                                                                                                                                    |
| -------------- | -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `CountryCode`  | `string` | One of the 249 officially assigned ISO 3166-1 alpha-2 codes, in upper case. User-assigned (`XK`), reserved (`UK`, `EU`) and withdrawn (`CS`) codes are rejected.         |
| `CurrencyCode` | `string` | A code on ISO 4217 list one (published 2026-09-17), in upper case, including funds codes, precious metals, `XTS` and `XXX`. Withdrawn codes (`HRK`, `BGN`) are rejected. |
| `LanguageCode` | `string` | A BCP 47 tag .NET knows as a predefined culture, written as .NET names it (`en-US`). Depends on the platform's ICU data; rejects everything in invariant globalization.  |
| `Iban`         | `string` | ISO 13616 electronic format (no spaces, upper case), 15 to 34 characters, check digits 02 to 98 and the MOD 97-10 check. The country's IBAN length is not checked.       |
| `Isbn`         | `string` | An ISBN-10 (check digit modulo 11, `X` for 10) or an ISBN-13 starting 978 or 979 (check digit modulo 10), without hyphens.                                               |
| `Vin`          | `string` | ISO 3779: 17 upper-case letters and digits without `I`, `O` or `Q`, and the North American check digit in position 9.                                                    |
| `IpAddress`    | `string` | IPv4 dotted decimal or IPv6 in the RFC 5952 canonical form (lower case, compressed). No zone identifiers or prefix lengths. `ToIPAddress()` returns an `IPAddress`.      |
| `MacAddress`   | `string` | An EUI-48 in the IEEE 802 form: six pairs of upper-case hexadecimal digits separated by hyphens (`00-1A-2B-3C-4D-5E`).                                                   |

### Numbers

| Type          | Value     | Rules                                         |
| ------------- | --------- | --------------------------------------------- |
| `Age`         | `int`     | 0 to 150 years.                               |
| `Percentage`  | `decimal` | 0 to 100.                                     |
| `Price`       | `decimal` | Zero or more; no currency, no rounding.       |
| `Discount`    | `decimal` | A percentage taken off, 0 to 100.             |
| `TaxRate`     | `decimal` | A percentage, 0 to 100.                       |
| `Quantity`    | `int`     | Zero or more.                                 |
| `Rating`      | `int`     | 1 to 5 (schema.org's default worst and best). |
| `Score`       | `int`     | 0 to 100.                                     |
| `Latitude`    | `double`  | Finite, -90 to 90 degrees.                    |
| `Longitude`   | `double`  | Finite, -180 to 180 degrees.                  |
| `Height`      | `double`  | Finite, zero or more, in metres.              |
| `Weight`      | `double`  | Finite, zero or more, in kilograms.           |
| `Distance`    | `double`  | Finite, zero or more, in metres.              |
| `Temperature` | `double`  | Finite, at least absolute zero (-273.15 °C).  |
| `Month`       | `int`     | 1 to 12.                                      |
| `Quarter`     | `int`     | 1 to 4.                                       |
| `Week`        | `int`     | An ISO 8601 week, 1 to 53.                    |
| `FiscalYear`  | `int`     | 1 to 9999, the years a `DateOnly` can hold.   |

### Dates

| Type             | Value      | Rules                                                                                                                                   |
| ---------------- | ---------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| `BirthDate`      | `DateOnly` | Not after today anywhere on Earth (UTC+14), and not more than 150 years ago. The bounds move with the clock.                            |
| `ExpirationDate` | `DateOnly` | Any date but `DateOnly.MinValue`; past dates are accepted so expired values still load. `IsExpiredOn(date)` is true from the day after. |
| `BusinessDate`   | `DateOnly` | Monday to Friday. Public holidays are not checked.                                                                                      |

## Ranges and money

Three records sit beside the strong types. They have the same surface: a static `From(...)` that throws for an invalid value, a
`TryFrom(..., out result)` that returns `false`, and a constructor that checks the same way, so reading one from JSON checks it too.
Each serializes as a JSON object.

| Type        | Members                                   | Rules                                                                                                 | JSON                                        |
| ----------- | ----------------------------------------- | ----------------------------------------------------------------------------------------------------- | ------------------------------------------- |
| `DateRange` | `DateOnly Start`, `DateOnly End`          | `Start` is not after `End`; both days are in the range (`Contains(date)`).                            | `{"start":"2024-01-01","end":"2024-01-31"}` |
| `TimeRange` | `TimeOnly Start`, `TimeOnly End`          | `Start` is not after `End`, so a range cannot run past midnight. `Duration` is `End - Start`.         | `{"start":"09:00:00","end":"17:30:00"}`     |
| `Money`     | `decimal Amount`, `CurrencyCode Currency` | The currency is required. The amount can be negative and is not rounded to the currency's minor unit. | `{"amount":12.5,"currency":"USD"}`          |

## Declaring your own strong type

Referencing Primitives brings SuperStrong's source generator and analyzers into the project, so an application declares its own
strong types the same way Primitives does. To reuse a ready-made rules set under a name of your own, give the rules set as the
second type argument:

```csharp
using Rocket.Surgery.LaunchPad.StrongTypes;
using SuperStrong.Types;

[StrongType<string, EmailAddressRules>]
public sealed partial class BillingEmail;
```

`BillingEmail` follows the rules of `EmailAddress` and is a type of its own: a `BillingEmail` is not an `EmailAddress`, so the two
cannot be mixed up.

For rules of your own, write a rules set: a class that implements `IStrongTypeTemplate<T>` and builds its rules with
`StrongType.Define<T>()`. A rules set can be shared by several strong types.

```csharp
public sealed class SkuRules : IStrongTypeTemplate<string>
{
    public static StrongTypeDefinition<string> Define() => StrongType
        .Define<string>()
        .HasLength(8)
        .MatchesRegex("^[A-Z]{3}-[0-9]{4}$")
        .Satisfies(value => !value.StartsWith("TST", StringComparison.Ordinal), "Test SKUs are not allowed.");
}

[StrongType<string, SkuRules>]
public sealed partial class Sku;
```

A strong type with rules only it needs can define them itself instead; see SuperStrong's
[validation](https://superstrong.dev/tutorial/validation) guide.

### JSON in the declaring project

The System.Text.Json source generator does not see the `[JsonConverter]` that SuperStrong generates in the same project, so a
`JsonSerializerContext` cannot list a strong type declared next to it: it would treat the type as an object. Declare strong types in
a library the application references (a context in the application sees the converter in the library's metadata, and so does the
JSON context LaunchPad writes for an executable), or register the type's `JsonStrongTypeConverter<TStrongType, TPrimitive>` yourself.
The ready-made types are declared in Primitives, so every JSON context sees their converters.
