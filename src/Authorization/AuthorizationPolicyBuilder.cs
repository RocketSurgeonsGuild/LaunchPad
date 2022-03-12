// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Rocket.Surgery.LaunchPad.Authorization;

/// <summary>
///     Used for building policies.
/// </summary>
public class AuthorizationPolicyBuilder
{
    /// <summary>
    ///     Creates a new instance of <see cref="AuthorizationPolicyBuilder" />
    /// </summary>
    public AuthorizationPolicyBuilder()
    {
    }

    /// <summary>
    ///     Creates a new instance of <see cref="AuthorizationPolicyBuilder" />.
    /// </summary>
    /// <param name="policy">The <see cref="AuthorizationPolicy" /> to copy.</param>
    public AuthorizationPolicyBuilder(AuthorizationPolicy policy)
    {
        Combine(policy);
    }

    /// <summary>
    ///     Gets or sets a list of <see cref="IAuthorizationRequirement" />s which must succeed for
    ///     this policy to be successful.
    /// </summary>
    public IList<IAuthorizationRequirement> Requirements { get; set; } = new List<IAuthorizationRequirement>();

    /// <summary>
    ///     Adds the specified <paramref name="requirements" /> to the
    ///     <see cref="AuthorizationPolicyBuilder.Requirements" /> for this instance.
    /// </summary>
    /// <param name="requirements">The authorization requirements to add.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public AuthorizationPolicyBuilder AddRequirements(params IAuthorizationRequirement[] requirements)
    {
        foreach (var req in requirements)
        {
            Requirements.Add(req);
        }

        return this;
    }

    /// <summary>
    ///     Combines the specified <paramref name="policy" /> into the current instance.
    /// </summary>
    /// <param name="policy">The <see cref="AuthorizationPolicy" /> to combine.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public AuthorizationPolicyBuilder Combine(AuthorizationPolicy policy)
    {
        if (policy == null)
        {
            throw new ArgumentNullException(nameof(policy));
        }

        AddRequirements(policy.Requirements.ToArray());
        return this;
    }

    /// <summary>
    ///     Adds a <see cref="ClaimsAuthorizationRequirement" /> to the current instance which requires
    ///     that the current user has the specified claim and that the claim value must be one of the allowed values.
    /// </summary>
    /// <param name="claimType">The claim type required.</param>
    /// <param name="allowedValues">Values the claim must process one or more of for evaluation to succeed.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public AuthorizationPolicyBuilder RequireClaim(string claimType, params string[] allowedValues)
    {
        if (claimType == null)
        {
            throw new ArgumentNullException(nameof(claimType));
        }

        return RequireClaim(claimType, (IEnumerable<string>)allowedValues);
    }

    /// <summary>
    ///     Adds a <see cref="ClaimsAuthorizationRequirement" /> to the current instance which requires
    ///     that the current user has the specified claim and that the claim value must be one of the allowed values.
    /// </summary>
    /// <param name="claimType">The claim type required.</param>
    /// <param name="allowedValues">Values the claim must process one or more of for evaluation to succeed.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public AuthorizationPolicyBuilder RequireClaim(string claimType, IEnumerable<string> allowedValues)
    {
        if (claimType == null)
        {
            throw new ArgumentNullException(nameof(claimType));
        }

        Requirements.Add(new ClaimsAuthorizationRequirement(claimType, allowedValues));
        return this;
    }

    /// <summary>
    ///     Adds a <see cref="ClaimsAuthorizationRequirement" /> to the current instance which requires
    ///     that the current user has the specified claim.
    /// </summary>
    /// <param name="claimType">The claim type required, with no restrictions on claim value.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public AuthorizationPolicyBuilder RequireClaim(string claimType)
    {
        if (claimType == null)
        {
            throw new ArgumentNullException(nameof(claimType));
        }

        Requirements.Add(new ClaimsAuthorizationRequirement(claimType, null));
        return this;
    }

    /// <summary>
    ///     Adds a <see cref="RolesAuthorizationRequirement" /> to the current instance which enforces that the current user
    ///     must have at least one of the specified roles.
    /// </summary>
    /// <param name="roles">The allowed roles.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public AuthorizationPolicyBuilder RequireRole(params string[] roles)
    {
        if (roles == null)
        {
            throw new ArgumentNullException(nameof(roles));
        }

        return RequireRole((IEnumerable<string>)roles);
    }

    /// <summary>
    ///     Adds a <see cref="RolesAuthorizationRequirement" /> to the current instance which enforces that the current user
    ///     must have at least one of the specified roles.
    /// </summary>
    /// <param name="roles">The allowed roles.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public AuthorizationPolicyBuilder RequireRole(IEnumerable<string> roles)
    {
        if (roles == null)
        {
            throw new ArgumentNullException(nameof(roles));
        }

        Requirements.Add(new RolesAuthorizationRequirement(roles));
        return this;
    }

    /// <summary>
    ///     Adds a <see cref="NameAuthorizationRequirement" /> to the current instance which enforces that the current user matches the specified name.
    /// </summary>
    /// <param name="userName">The user name the current user must possess.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public AuthorizationPolicyBuilder RequireUserName(string userName)
    {
        if (userName == null)
        {
            throw new ArgumentNullException(nameof(userName));
        }

        Requirements.Add(new NameAuthorizationRequirement(userName));
        return this;
    }

    /// <summary>
    ///     Adds <see cref="DenyAnonymousAuthorizationRequirement" /> to the current instance which enforces that the current user is authenticated.
    /// </summary>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public AuthorizationPolicyBuilder RequireAuthenticatedUser()
    {
        Requirements.Add(new DenyAnonymousAuthorizationRequirement());
        return this;
    }

    /// <summary>
    ///     Adds an <see cref="AssertionRequirement" /> to the current instance.
    /// </summary>
    /// <param name="handler">The handler to evaluate during authorization.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public AuthorizationPolicyBuilder RequireAssertion(Func<AuthorizationHandlerContext, bool> handler)
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        Requirements.Add(new AssertionRequirement(handler));
        return this;
    }

    /// <summary>
    ///     Adds an <see cref="AssertionRequirement" /> to the current instance.
    /// </summary>
    /// <param name="handler">The handler to evaluate during authorization.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public AuthorizationPolicyBuilder RequireAssertion(Func<AuthorizationHandlerContext, Task<bool>> handler)
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        Requirements.Add(new AssertionRequirement(handler));
        return this;
    }

    /// <summary>
    ///     Builds a new <see cref="AuthorizationPolicy" /> from the requirements
    ///     in this instance.
    /// </summary>
    /// <returns>
    ///     A new <see cref="AuthorizationPolicy" /> built from the requirements in this instance.
    /// </returns>
    public AuthorizationPolicy Build()
    {
        return new AuthorizationPolicy(Requirements);
    }
}
