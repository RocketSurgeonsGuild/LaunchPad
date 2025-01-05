using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.Primitives.Conventions;

/// <summary>
///     Service conventions using service discovery
/// </summary>
[PublicAPI]
[ExportConvention]
[ConventionCategory(ConventionCategory.Application)]
public class StandardResilienceConvention : IServiceConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services) => services.ConfigureHttpClientDefaults(http => http.AddStandardResilienceHandler());
}
