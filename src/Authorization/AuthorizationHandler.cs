// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Rocket.Surgery.LaunchPad.Authorization;

/// <summary>
///     Base class for authorization handlers that need to be called for a specific requirement type.
/// </summary>
/// <typeparam name="TRequirement">The type of the requirement to handle.</typeparam>
[PublicAPI]
public abstract class AuthorizationHandler<TRequirement> : IAuthorizationHandler
    where TRequirement : IAuthorizationRequirement
{
    /// <summary>
    ///     Makes a decision if authorization is allowed based on a specific requirement.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="requirement">The requirement to evaluate.</param>
    protected abstract Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement);

    /// <summary>
    ///     Makes a decision if authorization is allowed.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    public virtual async Task HandleAsync(AuthorizationHandlerContext context)
    {
        foreach (var req in context.Requirements.OfType<TRequirement>())
        {
            await HandleRequirementAsync(context, req).ConfigureAwait(false);
        }
    }
}

/// <summary>
///     Base class for authorization handlers that need to be called for specific requirement and
///     resource types.
/// </summary>
/// <typeparam name="TRequirement">The type of the requirement to evaluate.</typeparam>
/// <typeparam name="TResource">The type of the resource to evaluate.</typeparam>
public abstract class AuthorizationHandler<TRequirement, TResource> : IAuthorizationHandler
    where TRequirement : IAuthorizationRequirement
{
    /// <summary>
    ///     Makes a decision if authorization is allowed based on a specific requirement and resource.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="requirement">The requirement to evaluate.</param>
    /// <param name="resource">The resource to evaluate.</param>
    protected abstract Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement, TResource resource);

    /// <summary>
    ///     Makes a decision if authorization is allowed.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    public virtual async Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (context.Resource is TResource)
        {
            foreach (var req in context.Requirements.OfType<TRequirement>())
            {
                await HandleRequirementAsync(context, req, (TResource)context.Resource).ConfigureAwait(false);
            }
        }
    }
}
