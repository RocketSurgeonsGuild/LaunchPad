using System.Text.RegularExpressions;

using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a slug, the readable part of a URL path that names a resource, such as <c>hello-world-2</c>.
/// </summary>
/// <remarks>
///     No standard defines a slug. These rules take the form that needs no percent-encoding in any part of a URI (RFC 3986 §2.3):
///     <list type="bullet">
///         <item>Lower-case ASCII letters and digits, in groups separated by single hyphens; no leading or trailing hyphen.</item>
///         <item>At most 100 characters.</item>
///     </list>
/// </remarks>
public sealed partial class SlugRules : IStrongTypeTemplate<string>
{
    /// <summary>The longest slug allowed.</summary>
    public const int MaxLength = 100;

    /// <inheritdoc />
    public static StrongTypeDefinition<string> Define() => StrongType.Define<string>().HasMaxLength(MaxLength).MatchesRegex(Pattern());

    [GeneratedRegex(@"^[a-z0-9]+(?:-[a-z0-9]+)*\z", RegexOptions.CultureInvariant)]
    private static partial Regex Pattern();

    private SlugRules() { }
}

/// <summary>A URL slug, valid under <see cref="SlugRules" />.</summary>
[StrongType<string, SlugRules>]
public sealed partial class Slug;
