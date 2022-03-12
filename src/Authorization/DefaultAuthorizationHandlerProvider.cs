// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Rocket.Surgery.LaunchPad.Authorization;

/// <summary>
///     The default implementation of a handler provider,
///     which provides the <see cref="IAuthorizationHandler" />s for an authorization request.
/// </summary>
public class DefaultAuthorizationHandlerProvider : IAuthorizationHandlerProvider
{
    private readonly IEnumerable<IAuthorizationHandler> _handlers;

    /// <summary>
    ///     Creates a new instance of <see cref="DefaultAuthorizationHandlerProvider" />.
    /// </summary>
    /// <param name="handlers">The <see cref="IAuthorizationHandler" />s.</param>
    public DefaultAuthorizationHandlerProvider(IEnumerable<IAuthorizationHandler> handlers)
    {
        if (handlers == null)
        {
            throw new ArgumentNullException(nameof(handlers));
        }

        _handlers = handlers;
    }

    /// <inheritdoc />
    public Task<IEnumerable<IAuthorizationHandler>> GetHandlersAsync(AuthorizationHandlerContext context)
    {
        return Task.FromResult(_handlers);
    }
}
