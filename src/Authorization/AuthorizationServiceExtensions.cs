// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Claims;

namespace Rocket.Surgery.LaunchPad.Authorization;

/// <summary>
///     Extension methods for <see cref="IAuthorizationService" />.
/// </summary>
public static class AuthorizationServiceExtensions
{
    /// <summary>
    ///     Checks if a user meets a specific requirement for the specified resource
    /// </summary>
    /// <param name="service">The <see cref="IAuthorizationService" /> providing authorization.</param>
    /// <param name="user">The user to evaluate the policy against.</param>
    /// <param name="resource">The resource to evaluate the policy against.</param>
    /// <param name="requirement">The requirement to evaluate the policy against.</param>
    /// <returns>
    ///     A flag indicating whether requirement evaluation has succeeded or failed.
    ///     This value is
    ///     <value>true</value>
    ///     when the user fulfills the policy, otherwise
    ///     <value>false</value>
    ///     .
    /// </returns>
    public static Task<AuthorizationResult> AuthorizeAsync(
        this IAuthorizationService service, ClaimsPrincipal user, object? resource, IAuthorizationRequirement requirement
    )
    {
        if (service == null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        if (requirement == null)
        {
            throw new ArgumentNullException(nameof(requirement));
        }

        return service.AuthorizeAsync(user, resource, new[] { requirement });
    }

    /// <summary>
    ///     Checks if a user meets a specific authorization policy against the specified resource.
    /// </summary>
    /// <param name="service">The <see cref="IAuthorizationService" /> providing authorization.</param>
    /// <param name="user">The user to evaluate the policy against.</param>
    /// <param name="resource">The resource to evaluate the policy against.</param>
    /// <param name="policy">The policy to evaluate.</param>
    /// <returns>
    ///     A flag indicating whether policy evaluation has succeeded or failed.
    ///     This value is
    ///     <value>true</value>
    ///     when the user fulfills the policy, otherwise
    ///     <value>false</value>
    ///     .
    /// </returns>
    public static Task<AuthorizationResult> AuthorizeAsync(
        this IAuthorizationService service, ClaimsPrincipal user, object? resource, AuthorizationPolicy policy
    )
    {
        if (service == null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        if (policy == null)
        {
            throw new ArgumentNullException(nameof(policy));
        }

        return service.AuthorizeAsync(user, resource, policy.Requirements);
    }

    /// <summary>
    ///     Checks if a user meets a specific authorization policy against the specified resource.
    /// </summary>
    /// <param name="service">The <see cref="IAuthorizationService" /> providing authorization.</param>
    /// <param name="user">The user to evaluate the policy against.</param>
    /// <param name="policy">The policy to evaluate.</param>
    /// <returns>
    ///     A flag indicating whether policy evaluation has succeeded or failed.
    ///     This value is
    ///     <value>true</value>
    ///     when the user fulfills the policy, otherwise
    ///     <value>false</value>
    ///     .
    /// </returns>
    public static Task<AuthorizationResult> AuthorizeAsync(this IAuthorizationService service, ClaimsPrincipal user, AuthorizationPolicy policy)
    {
        if (service == null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        if (policy == null)
        {
            throw new ArgumentNullException(nameof(policy));
        }

        return service.AuthorizeAsync(user, null, policy);
    }

    /// <summary>
    ///     Checks if a user meets a specific authorization policy against the specified resource.
    /// </summary>
    /// <param name="service">The <see cref="IAuthorizationService" /> providing authorization.</param>
    /// <param name="user">The user to evaluate the policy against.</param>
    /// <param name="policyName">The name of the policy to evaluate.</param>
    /// <returns>
    ///     A flag indicating whether policy evaluation has succeeded or failed.
    ///     This value is
    ///     <value>true</value>
    ///     when the user fulfills the policy, otherwise
    ///     <value>false</value>
    ///     .
    /// </returns>
    public static Task<AuthorizationResult> AuthorizeAsync(this IAuthorizationService service, ClaimsPrincipal user, string policyName)
    {
        if (service == null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        if (policyName == null)
        {
            throw new ArgumentNullException(nameof(policyName));
        }

        return service.AuthorizeAsync(user, null, policyName);
    }
}
