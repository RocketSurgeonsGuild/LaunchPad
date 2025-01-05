using Microsoft.Extensions.Configuration;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;

using Serilog;

namespace Rocket.Surgery.LaunchPad.Serilog.Conventions;

/// <summary>
///     NodaTimeConvention.
/// </summary>
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
[ConventionCategory(ConventionCategory.Core)]
public class SerilogTimeConvention : ISerilogConvention
{
    /// <inheritdoc />
    public void Register(IConventionContext context, IConfiguration configuration, IServiceProvider services, LoggerConfiguration loggerConfiguration) => loggerConfiguration.Destructure.NodaTimeTypes();
}
