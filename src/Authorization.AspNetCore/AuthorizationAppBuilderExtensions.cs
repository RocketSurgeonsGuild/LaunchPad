// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Authorization;

/// <summary>
///     Extension methods to add authorization capabilities to an HTTP application pipeline.
/// </summary>
public static class AuthorizationAppBuilderExtensions
{
    /// <summary>
    ///     Adds the <see cref="AuthorizationMiddleware" /> to the specified <see cref="IApplicationBuilder" />, which enables authorization capabilities.
    ///     <para>
    ///         When authorizing a resource that is routed using endpoint routing, this call must appear between the calls to
    ///         <c>app.UseRouting()</c> and <c>app.UseEndpoints(...)</c> for the middleware to function correctly.
    ///     </para>
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder" /> to add the middleware to.</param>
    /// <returns>A reference to <paramref name="app" /> after the operation has completed.</returns>
    public static IApplicationBuilder UseAuthorization(this IApplicationBuilder app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        VerifyServicesRegistered(app);

        return app.UseMiddleware<AuthorizationMiddleware>();
    }

    private static void VerifyServicesRegistered(IApplicationBuilder app)
    {
        // Verify that AddAuthorizationPolicy was called before calling UseAuthorization
        // We use the AuthorizationPolicyMarkerService to ensure all the services were added.
        if (app.ApplicationServices.GetService(typeof(AuthorizationPolicyMarkerService)) == null)
        {
            throw new InvalidOperationException(
                Resources.FormatException_UnableToFindServices(
                    nameof(IServiceCollection),
                    nameof(PolicyServiceCollectionExtensions.AddAuthorization)
                )
            );
        }
    }
}
