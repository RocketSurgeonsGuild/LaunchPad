using Microsoft.Extensions.Configuration;
using Serilog;
// ReSharper disable once CheckNamespace
using Rocket.Surgery.LaunchPad.Serilog;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions;

/// <summary>
///     Helper method for working with <see cref="ConventionContextBuilder" />
/// </summary>
[PublicAPI]
public static class SerilogAbstractionsHostBuilderExtensions
{
    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureSerilog(this ConventionContextBuilder container, Action<LoggerConfiguration> @delegate)
        => ConfigureSerilog(container, (_, _, _, logger) => @delegate(logger));

    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureSerilog(
        this ConventionContextBuilder container,
        Action<IConfiguration, IServiceProvider, LoggerConfiguration> @delegate
    ) => ConfigureSerilog(container, (_, configuration, services, logger) => @delegate(configuration, services, logger));

    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureSerilog(
        this ConventionContextBuilder container,
        Action<IServiceProvider, LoggerConfiguration> @delegate
    ) => ConfigureSerilog(container, (_, _, services, logger) => @delegate(services, logger));

    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureSerilog(this ConventionContextBuilder container, SerilogConvention @delegate)
    {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(@delegate);

        container.AppendDelegate(@delegate);
        return container;
    }
}
