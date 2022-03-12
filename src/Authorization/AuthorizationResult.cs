// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Claims;

namespace Rocket.Surgery.LaunchPad.Authorization;

/// <summary>
///     Encapsulates the result of <see cref="IAuthorizationService.AuthorizeAsync(ClaimsPrincipal, object, IEnumerable{IAuthorizationRequirement})" />.
/// </summary>
public class AuthorizationResult
{
    /// <summary>
    ///     Returns a successful result.
    /// </summary>
    /// <returns>A successful result.</returns>
    public static AuthorizationResult Success()
    {
        return new AuthorizationResult { Succeeded = true };
    }

    /// <summary>
    ///     Creates a failed authorization result.
    /// </summary>
    /// <param name="failure">Contains information about why authorization failed.</param>
    /// <returns>The <see cref="AuthorizationResult" />.</returns>
    public static AuthorizationResult Failed(AuthorizationFailure failure)
    {
        return new AuthorizationResult { Failure = failure };
    }

    /// <summary>
    ///     Creates a failed authorization result.
    /// </summary>
    /// <returns>The <see cref="AuthorizationResult" />.</returns>
    public static AuthorizationResult Failed()
    {
        return new AuthorizationResult { Failure = AuthorizationFailure.ExplicitFail() };
    }

    private AuthorizationResult()
    {
    }

    /// <summary>
    ///     True if authorization was successful.
    /// </summary>
    public bool Succeeded { get; private set; }

    /// <summary>
    ///     Contains information about why authorization failed.
    /// </summary>
    public AuthorizationFailure? Failure { get; private set; }
}
