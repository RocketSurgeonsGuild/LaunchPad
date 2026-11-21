using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Authorization;

/// <summary>Resolves a policy class for a call. Called by generated code.</summary>
[PublicAPI]
public static class PolicyResolver
{
    /// <summary>
    ///     The policy from the services of the call when it is registered (as <c>AddZeroAllocAuthorization()</c> does), otherwise a new one
    ///     with its dependencies from them.
    /// </summary>
    /// <typeparam name="TPolicy">The policy class.</typeparam>
    /// <param name="services">The services of the call.</param>
    public static TPolicy Resolve<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TPolicy>(IServiceProvider services)
        where TPolicy : notnull =>
        services.GetService<TPolicy>() ?? ActivatorUtilities.CreateInstance<TPolicy>(services);
}
