// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Rocket.Surgery.LaunchPad.Authorization;

/// <summary>
///     Default implementation for <see cref="IAuthorizationMiddlewareResultHandler" />.
/// </summary>
public class AuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    /// <inheritdoc />
    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Challenged)
        {
            if (policy.AuthenticationSchemes.Count > 0)
            {
                foreach (var scheme in policy.AuthenticationSchemes)
                {
                    await context.ChallengeAsync(scheme);
                }
            }
            else
            {
                await context.ChallengeAsync();
            }

            return;
        }

        if (authorizeResult.Forbidden)
        {
            if (policy.AuthenticationSchemes.Count > 0)
            {
                foreach (var scheme in policy.AuthenticationSchemes)
                {
                    await context.ForbidAsync(scheme);
                }
            }
            else
            {
                await context.ForbidAsync();
            }

            return;
        }

        await next(context);
    }
}
