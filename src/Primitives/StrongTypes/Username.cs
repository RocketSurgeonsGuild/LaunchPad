using System.Text.RegularExpressions;

using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a username, the name a person signs in with.
/// </summary>
/// <remarks>
///     No standard fixes a username's form; RFC 8265 (PRECIS) allows almost any Unicode letter. These rules take an ASCII subset of it that
///     is safe in URLs, file names and mentions:
///     <list type="bullet">
///         <item>3 to 32 characters.</item>
///         <item>
///             ASCII letters and digits, with single dots, underscores or hyphens between them: it starts and ends with a letter or digit,
///             and has no two separators in a row.
///         </item>
///         <item>The value is kept as given; usernames that differ only in case are different values.</item>
///     </list>
/// </remarks>
public sealed partial class UsernameRules : IStrongTypeTemplate<string>
{
    /// <summary>The shortest username allowed.</summary>
    public const int MinLength = 3;

    /// <summary>The longest username allowed.</summary>
    public const int MaxLength = 32;

    /// <inheritdoc />
    public static StrongTypeDefinition<string> Define() => StrongType
                                                         .Define<string>()
                                                         .HasMinLength(MinLength)
                                                         .HasMaxLength(MaxLength)
                                                         .MatchesRegex(Pattern());

    [GeneratedRegex(@"^[A-Za-z0-9](?:[._-]?[A-Za-z0-9])*\z", RegexOptions.CultureInvariant)]
    private static partial Regex Pattern();

    private UsernameRules() { }
}

/// <summary>A username, valid under <see cref="UsernameRules" />.</summary>
[StrongType<string, UsernameRules>]
public sealed partial class Username;
