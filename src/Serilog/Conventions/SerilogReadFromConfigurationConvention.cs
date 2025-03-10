using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration;
using Serilog;
using Serilog.Extensions.Logging;

namespace Rocket.Surgery.LaunchPad.Serilog.Conventions;

/// <summary>
///     SerilogReadFromConfigurationConvention.
///     Implements the <see cref="ISerilogConvention" />
/// </summary>
/// <seealso cref="ISerilogConvention" />
[PublicAPI]
[LiveConvention]
[ExportConvention]
[ConventionCategory(ConventionCategory.Core)]
public class SerilogReadFromConfigurationConvention : ISerilogConvention, IConfigurationConvention
{
    /// <inheritdoc />
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = "The type is an enum value"
    )]
    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2066:The generic parameter of type or method has a DynamicallyAccessedMembersAttribute, but the value used for it can not be statically analyzed.",
        Justification = "The type is an enum value"
    )]
    public void Register(IConventionContext context, IConfiguration configuration, IConfigurationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(context);

        var applicationLogLevel = configuration.GetValue<LogLevel?>("ApplicationState:LogLevel");
        if (!applicationLogLevel.HasValue) return;

        builder.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                {
                    "Serilog:MinimumLevel:Default",
                    LevelConvert.ToSerilogLevel(applicationLogLevel.Value).ToString()
                },
            }
        );
    }

    /// <inheritdoc />
    public void Register(
        IConventionContext context,
        IConfiguration configuration,
        IServiceProvider services,
        LoggerConfiguration loggerConfiguration
    )
    {
        ArgumentNullException.ThrowIfNull(context);

        loggerConfiguration.ReadFrom.Configuration(configuration);
    }
}
