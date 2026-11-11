using System.Text.RegularExpressions;

using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a URL: an absolute <c>http</c> or <c>https</c> URI (RFC 3986 §4.3) with a host.
/// </summary>
/// <remarks>
///     <list type="bullet">
///         <item>The scheme is written in lower case, the form RFC 3986 §3.1 says to produce; <c>HTTP://</c> is rejected.</item>
///         <item>
///             Only the characters RFC 3986 allows in a URI: unreserved and reserved characters, and <c>%</c> followed by two hexadecimal
///             digits. Spaces and other characters must already be percent-encoded, and internationalized host names written as A-labels.
///         </item>
///         <item>At most 8000 characters, the length RFC 9110 §4.1 recommends every recipient supports.</item>
///         <item>The value is kept as given; it is not normalized.</item>
///     </list>
/// </remarks>
public sealed partial class UrlRules : IStrongTypeTemplate<string>
{
    /// <summary>The longest URL allowed.</summary>
    public const int MaxLength = 8000;

    /// <inheritdoc />
    public static StrongTypeDefinition<string> Define() => StrongType
                                                         .Define<string>()
                                                         .HasMaxLength(MaxLength)
                                                         .MatchesRegex(Pattern())
                                                         .Satisfies(IsAbsoluteHttpUri, "Value must be an absolute http or https URI with a host.");

    private static bool IsAbsoluteHttpUri(string value) =>
        Uri.TryCreate(value, UriKind.Absolute, out var uri)
     && ( uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps )
     && uri.Host.Length > 0;

    [GeneratedRegex(@"^https?://(?:[A-Za-z0-9\-._~:/?#\[\]@!$&'()*+,;=]|%[0-9A-Fa-f]{2})+\z", RegexOptions.CultureInvariant)]
    private static partial Regex Pattern();

    private UrlRules() { }
}

/// <summary>An absolute http or https URL, valid under <see cref="UrlRules" />.</summary>
[StrongType<string, UrlRules>]
public sealed partial class Url;
