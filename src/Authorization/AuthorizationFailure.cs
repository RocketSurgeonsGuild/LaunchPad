// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Claims;

namespace Rocket.Surgery.LaunchPad.Authorization;

/// <summary>
///     Encapsulates a failure result of <see cref="IAuthorizationService.AuthorizeAsync(ClaimsPrincipal, object, IEnumerable{IAuthorizationRequirement})" />.
/// </summary>
[PublicAPI]
public class AuthorizationFailure
{
    /// <summary>
    ///     Return a failure due to <see cref="AuthorizationHandlerContext.Fail()" /> being called.
    /// </summary>
    /// <returns>The failure.</returns>
    public static AuthorizationFailure ExplicitFail()
    {
        return new AuthorizationFailure
        {
            FailCalled = true
        };
    }

    /// <summary>
    ///     Return a failure due to <see cref="AuthorizationHandlerContext.Fail(AuthorizationFailureReason)" /> being called.
    /// </summary>
    /// <returns>The failure.</returns>
    public static AuthorizationFailure Failed(IEnumerable<AuthorizationFailureReason> reasons)
    {
        return new AuthorizationFailure
        {
            FailCalled = true,
            FailureReasons = reasons
        };
    }

    /// <summary>
    ///     Return a failure due to some requirements not being met via <see cref="AuthorizationHandlerContext.Succeed(IAuthorizationRequirement)" />.
    /// </summary>
    /// <param name="failed">The requirements that were not met.</param>
    /// <returns>The failure.</returns>
    public static AuthorizationFailure Failed(IEnumerable<IAuthorizationRequirement> failed)
    {
        return new AuthorizationFailure { FailedRequirements = failed };
    }

    private AuthorizationFailure()
    {
    }

    /// <summary>
    ///     Failure was due to <see cref="AuthorizationHandlerContext.Fail()" /> being called.
    /// </summary>
    public bool FailCalled { get; private set; }

    /// <summary>
    ///     Failure was due to these requirements not being met via <see cref="AuthorizationHandlerContext.Succeed(IAuthorizationRequirement)" />.
    /// </summary>
    public IEnumerable<IAuthorizationRequirement> FailedRequirements { get; private set; } = Array.Empty<IAuthorizationRequirement>();

    /// <summary>
    ///     Allows <see cref="IAuthorizationHandler" /> to flow more detailed reasons for why authorization failed.
    /// </summary>
    public IEnumerable<AuthorizationFailureReason> FailureReasons { get; private set; } = Array.Empty<AuthorizationFailureReason>();
}
