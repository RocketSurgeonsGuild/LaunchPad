using System.Text.RegularExpressions;

using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of an email address: an RFC 5322 <c>addr-spec</c> in the form RFC 5321 delivers mail to.
/// </summary>
/// <remarks>
///     <list type="bullet">
///         <item>
///             At most 254 characters (the RFC 5321 §4.5.3.1.3 path limit of 256 less its angle brackets), with a local part of at most 64
///             (RFC 5321 §4.5.3.1.1).
///         </item>
///         <item>
///             The local part is a <c>dot-atom</c> (RFC 5322 §3.4.1, §3.2.3): letters, digits and <c>!#$%&amp;'*+/=?^_`{|}~-</c>, in runs
///             separated by single dots. Quoted local parts, comments and the obsolete syntax are rejected.
///         </item>
///         <item>
///             The domain is a host name of at least two labels (RFC 1035 §2.3.1, RFC 1123 §2.1): each 1 to 63 letters, digits and hyphens
///             that neither starts nor ends with a hyphen, and a top-level label that starts with a letter (RFC 3696 §2). Address literals
///             (<c>[192.0.2.1]</c>) and single-label domains (<c>user@localhost</c>) are rejected.
///         </item>
///         <item>
///             ASCII only: an internationalized domain is written as its A-label (<c>xn--</c>), and internationalized local parts (RFC 6531)
///             are rejected.
///         </item>
///         <item>
///             The value is kept as given. The local part is case-sensitive (RFC 5321 §2.4), so addresses that differ only in case are
///             different values.
///         </item>
///     </list>
/// </remarks>
[StrongTypeFormat("email")]
public sealed partial class EmailAddressRules : IStrongTypeTemplate<string>
{
    /// <summary>The longest email address RFC 5321 can deliver to.</summary>
    public const int MaxLength = 254;

    /// <summary>The longest local part RFC 5321 allows.</summary>
    public const int MaxLocalPartLength = 64;

    /// <inheritdoc />
    public static StrongTypeDefinition<string> Define() => StrongType
                                                         .Define<string>()
                                                         .HasMaxLength(MaxLength)
                                                         .Satisfies(
                                                              static value => value.IndexOf('@', StringComparison.Ordinal) is var at
                                                               && at <= MaxLocalPartLength,
                                                              $"The local part must be at most {MaxLocalPartLength} characters."
                                                          )
                                                         .MatchesRegex(Pattern());

    [GeneratedRegex(
        """^[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?\.)+[A-Za-z](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?\z""",
        RegexOptions.CultureInvariant
    )]
    private static partial Regex Pattern();

    private EmailAddressRules() { }
}

/// <summary>An email address, valid under <see cref="EmailAddressRules" />.</summary>
[StrongType<string, EmailAddressRules>]
public sealed partial class EmailAddress;
