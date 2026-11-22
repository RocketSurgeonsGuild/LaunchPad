using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Rocket.Surgery.LaunchPad.Authorization;

/// <summary>
///     Registers what authorizes requests by their policies. LaunchPad's generator writes the calls for the requests of each project, into
///     the <c>AddLaunchPadRequestAuthorization()</c> its joint applies; an application without Dovetail calls that method itself.
/// </summary>
[PublicAPI]
public static class AuthorizationServiceCollectionExtensions
{
    /// <summary>
    ///     Registers what every plan needs: the <see cref="RequestAuthorizer" /> and the <see cref="AuthorizationOutcomes" /> of each call,
    ///     and the <see cref="RequestAuthorizationPlans" /> of the application.
    /// </summary>
    /// <param name="services">The services.</param>
    public static IServiceCollection AddLaunchPadAuthorization(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.TryAddSingleton<RequestAuthorizationPlans>();
        services.TryAddScoped<AuthorizationOutcomes>();
        services.TryAddScoped<RequestAuthorizer>();
        return services;
    }

    /// <summary>Registers the plan of a request, unless it has one.</summary>
    /// <typeparam name="TRequest">The request.</typeparam>
    /// <param name="services">The services.</param>
    /// <param name="groups">The requirements, all of which must pass.</param>
    public static IServiceCollection AddRequestAuthorizationPlan<TRequest>(this IServiceCollection services, params PolicyGroup[] groups)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddLaunchPadAuthorization();
        if (services.Any(d => d.ServiceType == typeof(RequestAuthorizationPlan<TRequest>))) return services;

        var plan = new RequestAuthorizationPlan<TRequest>(groups);
        services.AddSingleton(plan);
        // The edge finds a plan by name, among all of them.
        services.AddSingleton<RequestAuthorizationPlan>(plan);
        return services;
    }

    /// <summary>Registers how a denial becomes a response, unless the response has it registered.</summary>
    /// <typeparam name="TResponse">The response union.</typeparam>
    /// <param name="services">The services.</param>
    /// <param name="unauthorized">Converts <see cref="Unauthorized" /> to the response.</param>
    /// <param name="forbidden">Converts <see cref="Forbidden" /> to the response.</param>
    public static IServiceCollection AddAuthorizationResponse<TResponse>(
        this IServiceCollection services,
        Func<Unauthorized, TResponse> unauthorized,
        Func<Forbidden, TResponse> forbidden
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        services.TryAddSingleton(new AuthorizationResponse<TResponse>(unauthorized, forbidden));
        return services;
    }
}
