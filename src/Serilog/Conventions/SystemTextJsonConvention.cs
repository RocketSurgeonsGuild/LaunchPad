using Microsoft.Extensions.Configuration;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;

using Serilog;

namespace Rocket.Surgery.LaunchPad.Serilog.Conventions;

/// <summary>
///     MediatRConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
[ConventionCategory(ConventionCategory.Core)]
public class SerilogSystemTextJsonConvention : ISerilogConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceProvider services, LoggerConfiguration loggerConfiguration) => loggerConfiguration.Destructure.SystemTextJsonTypes();
}
