using Rocket.Surgery.LaunchPad.Primitives;

namespace Primitives.Behavior.Tests;

// String domain primitives, completed by the Primitives generator: each rule as a struct and as a class.

[StringPrimitive, Trim, MinLength(1), MaxLength(100)]
public readonly partial record struct FirstName;

[StringPrimitive, Trim, MinLength(1), MaxLength(100)]
public sealed partial record LastName;

[StringPrimitive, Trim, UpperCase, NormalizeWhitespace, NotEmpty]
public readonly partial record struct DisplayName;

[StringPrimitive, Trim, LowerCase, MinLength(1), MaxLength(254)]
public sealed partial record Tag;

[StringPrimitive, TrimStart]
public readonly partial record struct Indented;

[StringPrimitive, TrimEnd]
public readonly partial record struct Ragged;

[StringPrimitive, Trim, Regex(@"^[A-Z]{2}-\d{4}$", ErrorMessage = "Must be in format XX-0000")]
public readonly partial record struct ProductCode;

[StringPrimitive, Regex("^[a-z]+$", ErrorCode = "LOWER_ONLY"), Regex("^.{3,}$", ErrorCode = "TOO_SHORT", ErrorMessage = "Three or more.")]
public sealed partial record Word;

[StringPrimitive, NotEmpty(ErrorCode = "REQUIRED", ErrorMessage = "A secret is required."), MinLength(32, ErrorCode = "WEAK")]
public readonly partial record struct ApiSecret;

[StringPrimitive, ExactLength(4)]
public readonly partial record struct Pin;

[StringPrimitive, Length(2, 5)]
public sealed partial record Initials;

[StringPrimitive]
public readonly partial record struct Unconstrained;

[StringPrimitive, Normalize<ShoutNormalizer>, CustomValidator<NoLetterXValidator>]
public readonly partial record struct Shout;

[StringPrimitive, Trim, Normalize<ShoutNormalizer>, CustomValidator<NoLetterXValidator>]
public sealed partial record ShoutClass;

/// <summary>Upper cases the value and adds an exclamation mark, once.</summary>
public sealed class ShoutNormalizer : INormalizer<string>
{
    public static string Normalize(string value) => value.ToUpperInvariant().TrimEnd('!') + "!";
}

/// <summary>Rejects an X.</summary>
public sealed class NoLetterXValidator : ICustomValidator<string>
{
    public static PrimitiveError Validate(string value) => value.Contains('X', StringComparison.Ordinal) ? new("NO_X", "No X allowed.") : PrimitiveError.None;
}

// The string kinds.

[Email]
public readonly partial record struct EmailAddress;

[Email(MaxLength = 30)]
public sealed partial record ShortEmail;

[Phone]
public readonly partial record struct PhoneNumber;

[Url]
public sealed partial record Website;

[Url(AllowedSchemes = ["https", "ftp"])]
public readonly partial record struct SecureFtpUrl;

[Slug]
public readonly partial record struct ArticleSlug;

[CountryCode]
public readonly partial record struct Country;

[CountryCode]
public sealed partial record CountryClass;

[LanguageCode]
public readonly partial record struct Language;

[CurrencyCode]
public sealed partial record Currency;

[Username]
public readonly partial record struct UserName;

[Username(MinLength = 5, MaxLength = 8, ErrorCode = "HANDLE")]
public sealed partial record Handle;

[PasswordHash]
public readonly partial record struct PasswordHashValue;

[HexColor]
public readonly partial record struct ThemeColor;

[IPAddress]
public readonly partial record struct ClientIp;

[MacAddress]
public sealed partial record DeviceMac;

[IBAN]
public readonly partial record struct BankAccount;

[ISBN]
public sealed partial record BookIsbn;

[VIN]
public readonly partial record struct VehicleVin;

public static partial class Catalog
{
    public partial record Product
    {
        [Slug]
        public readonly partial record struct NestedSlug;
    }
}
