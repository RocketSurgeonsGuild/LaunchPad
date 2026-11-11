using SuperStrong.Types;

namespace Rocket.Surgery.LaunchPad.StrongTypes;

/// <summary>
///     The rules of a password hash: the stored, opaque output of a password hashing function, never the password itself.
/// </summary>
/// <remarks>
///     The format belongs to the hashing function (a PHC string, bcrypt's modular crypt format, ASP.NET Core Identity's base64), so the
///     only rule is that the value is not empty or white space.
/// </remarks>
public sealed class PasswordHashRules : IStrongTypeTemplate<string>
{
    /// <inheritdoc />
    public static StrongTypeDefinition<string> Define() => StrongType.Define<string>().IsNotEmpty();

    private PasswordHashRules() { }
}

/// <summary>A password hash, valid under <see cref="PasswordHashRules" />.</summary>
/// <remarks>
///     <see cref="ToString" /> does not write the hash, so it stays out of logs and messages; <see cref="AsPrimitive" />, JSON and type
///     conversion use the value.
/// </remarks>
[StrongType<string, PasswordHashRules>]
public sealed partial class PasswordHash
{
    /// <summary>Returns a placeholder instead of the hash.</summary>
    public override string ToString() => "***";
}
