using Microsoft.Extensions.Configuration;
using Rocket.Surgery.LaunchPad.Serilog;
using Serilog;

// ReSharper disable once CheckNamespace

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
    /// <param name="priority"></param>
    /// <param name="category"></param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureSerilog(
        this ConventionContextBuilder container,
        Action<LoggerConfiguration> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        return ConfigureSerilog(container, (_, _, _, logger) => @delegate(logger), priority, category);
    }

    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority"></param>
    /// <param name="category"></param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureSerilog(
        this ConventionContextBuilder container,
        Action<IConfiguration, IServiceProvider, LoggerConfiguration> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        return ConfigureSerilog(container, (_, configuration, services, logger) => @delegate(configuration, services, logger), priority, category);
    }

    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority"></param>
    /// <param name="category"></param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureSerilog(
        this ConventionContextBuilder container,
        Action<IServiceProvider, LoggerConfiguration> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        return ConfigureSerilog(container, (_, _, services, logger) => @delegate(services, logger), priority, category);
    }

    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority"></param>
    /// <param name="category"></param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureSerilog(
        this ConventionContextBuilder container,
        SerilogConvention @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(@delegate);

        container.AppendDelegate(@delegate, priority, category);
        return container;
    }
}
