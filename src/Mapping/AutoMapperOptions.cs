using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Mapping;

/// <summary>
///     Class AutoMapperOptions.
/// </summary>
public class AutoMapperOptions
{
    /// <summary>
    ///     Gets or sets the service lifetime.
    /// </summary>
    /// <value>The service lifetime.</value>
    public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Transient;
}
