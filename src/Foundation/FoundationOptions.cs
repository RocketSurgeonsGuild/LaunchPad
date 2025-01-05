using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Foundation;

/// <summary>
///     Common foundation options
/// </summary>
[PublicAPI]
public class FoundationOptions
{
    /// <summary>
    ///     The Mediator lifetime
    /// </summary>
    public ServiceLifetime MediatorLifetime { get; set; } = ServiceLifetime.Transient;

    /// <summary>
    ///     The Validator lifetime
    /// </summary>
    public ServiceLifetime ValidatorLifetime { get; set; } = ServiceLifetime.Singleton;

    /// <summary>
    ///     Validation options are registered as health checks instead of throwing and stopping application startup
    /// </summary>
    public bool? RegisterValidationOptionsAsHealthChecks { get; set; }
}
