using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Rocket.Surgery.LaunchPad.Primitives.Advanced;

/// <summary>
///     The patterns of the primitive kinds, which the code the generator writes for a kind matches the value against.
/// </summary>
/// <remarks>
///     They are here, built by the regular expression generator when Primitives is built, because a <c>[GeneratedRegex]</c> the Primitives
///     generator wrote into an application would never be implemented: one source generator doesn't see what another writes. Each has a one
///     second timeout: a match that times out rejects the value.
/// </remarks>
[PublicAPI]
[EditorBrowsable(EditorBrowsableState.Never)]
public static partial class PrimitivePatterns
{
    private const RegexOptions Options = RegexOptions.CultureInvariant;
    private const int Timeout = 1000;

    /// <summary>An email address: <c>[Email]</c>.</summary>
    [GeneratedRegex(
        """^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$""",
        Options,
        Timeout
    )]
    public static partial Regex Email { get; }

    /// <summary>An E.164 phone number: <c>[Phone]</c>.</summary>
    [GeneratedRegex("""^\+[1-9]\d{1,14}$""", Options, Timeout)]
    public static partial Regex Phone { get; }

    /// <summary>A URL slug: <c>[Slug]</c>.</summary>
    [GeneratedRegex("""^[a-z0-9]+(-[a-z0-9]+)*$""", Options, Timeout)]
    public static partial Regex Slug { get; }

    /// <summary>An ISO 639-1 language code: <c>[LanguageCode]</c>.</summary>
    [GeneratedRegex("""^[a-z]{2}$""", Options, Timeout)]
    public static partial Regex LanguageCode { get; }

    /// <summary>A user name: <c>[Username]</c>.</summary>
    [GeneratedRegex("""^[a-zA-Z0-9._-]+$""", Options, Timeout)]
    public static partial Regex Username { get; }

    /// <summary>A hex color, upper case: <c>[HexColor]</c>.</summary>
    [GeneratedRegex("""^#([0-9A-F]{3}|[0-9A-F]{4}|[0-9A-F]{6}|[0-9A-F]{8})$""", Options, Timeout)]
    public static partial Regex HexColor { get; }

    /// <summary>An IPv4 or IPv6 address: <c>[IPAddress]</c>.</summary>
    [GeneratedRegex(
        """^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$|^(?:[0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}$|^(?:[0-9a-fA-F]{1,4}:){1,7}:$|^:(?::[0-9a-fA-F]{1,4}){1,7}$|^(?:[0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}$|^(?:[0-9a-fA-F]{1,4}:){1,5}(?::[0-9a-fA-F]{1,4}){1,2}$|^(?:[0-9a-fA-F]{1,4}:){1,4}(?::[0-9a-fA-F]{1,4}){1,3}$|^(?:[0-9a-fA-F]{1,4}:){1,3}(?::[0-9a-fA-F]{1,4}){1,4}$|^(?:[0-9a-fA-F]{1,4}:){1,2}(?::[0-9a-fA-F]{1,4}){1,5}$|^[0-9a-fA-F]{1,4}:(?::[0-9a-fA-F]{1,4}){1,6}$|^::$""",
        Options,
        Timeout
    )]
    public static partial Regex IPAddress { get; }

    /// <summary>A MAC address, upper case: <c>[MacAddress]</c>.</summary>
    [GeneratedRegex("""^([0-9A-F]{2}[:-]){5}([0-9A-F]{2})$""", Options, Timeout)]
    public static partial Regex MacAddress { get; }

    /// <summary>The shape of an IBAN, upper case, before its check digits are checked: <c>[IBAN]</c>.</summary>
    [GeneratedRegex("""^[A-Z]{2}[0-9]{2}[A-Z0-9]{11,30}$""", Options, Timeout)]
    public static partial Regex Iban { get; }

    /// <summary>An ISBN-10 or ISBN-13, with or without its prefix and separators: <c>[ISBN]</c>.</summary>
    [GeneratedRegex(
        """^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0-9X]{13}$|97[89][0-9]{10}$|(?=(?:[0-9]+[- ]){4})[- 0-9]{17}$)(?:97[89][- ]?)?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]$""",
        Options,
        Timeout
    )]
    public static partial Regex Isbn { get; }

    /// <summary>A vehicle identification number, upper case: <c>[VIN]</c>.</summary>
    [GeneratedRegex("""^[A-HJ-NPR-Z0-9]{17}$""", Options, Timeout)]
    public static partial Regex Vin { get; }

    /// <summary>A run of white space, which <c>[NormalizeWhitespace]</c> replaces with one space.</summary>
    [GeneratedRegex("""\s+""", Options, Timeout)]
    public static partial Regex WhiteSpace { get; }
}
