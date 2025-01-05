using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.Telemetry;

/// <summary>
///     Extension method to apply configuration conventions
/// </summary>
[PublicAPI]
public static class RocketSurgeryOpenTelemetryExtensions
{
    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority"></param>
    /// <param name="category"></param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureOpenTelemetry(
        this ConventionContextBuilder container,
        OpenTelemetryConvention @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(@delegate);

        container.AppendDelegate(@delegate, priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority"></param>
    /// <param name="category"></param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureOpenTelemetry(
        this ConventionContextBuilder container,
        Action<IOpenTelemetryBuilder> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(@delegate);

        container.AppendDelegate(@delegate, priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority"></param>
    /// <param name="category"></param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureOpenTelemetryLogging(
        this ConventionContextBuilder container,
        OpenTelemetryLoggingConvention @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(@delegate);

        container.AppendDelegate(@delegate, priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority"></param>
    /// <param name="category"></param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureOpenTelemetryLogging(
        this ConventionContextBuilder container,
        Action<LoggerProviderBuilder> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(@delegate);

        container.AppendDelegate(@delegate, priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority"></param>
    /// <param name="category"></param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureOpenTelemetryMetrics(
        this ConventionContextBuilder container,
        OpenTelemetryMetricsConvention @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(@delegate);

        container.AppendDelegate(@delegate, priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority"></param>
    /// <param name="category"></param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureOpenTelemetryMetrics(
        this ConventionContextBuilder container,
        Action<MeterProviderBuilder> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(@delegate);

        container.AppendDelegate(@delegate, priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority"></param>
    /// <param name="category"></param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureOpenTelemetryTracing(
        this ConventionContextBuilder container,
        OpenTelemetryTracingConvention @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(@delegate);

        container.AppendDelegate(@delegate, priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority"></param>
    /// <param name="category"></param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureOpenTelemetryTracing(
        this ConventionContextBuilder container,
        Action<TracerProviderBuilder> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(@delegate);

        container.AppendDelegate(@delegate, priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority"></param>
    /// <param name="category"></param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureOpenTelemetry(
        this ConventionContextBuilder container,
        OpenTelemetryAsyncConvention @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(@delegate);

        container.AppendDelegate(@delegate, priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority"></param>
    /// <param name="category"></param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureOpenTelemetry(
        this ConventionContextBuilder container,
        Func<IOpenTelemetryBuilder, ValueTask> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(@delegate);

        container.AppendDelegate(new OpenTelemetryAsyncConvention((_, _, builder, _) => @delegate(builder)), priority, category);
        return container;
    }

    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <param name="priority"></param>
    /// <param name="category"></param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureOpenTelemetry(
        this ConventionContextBuilder container,
        Func<IOpenTelemetryBuilder, CancellationToken, ValueTask> @delegate,
        int priority = 0,
        ConventionCategory? category = null
    )
    {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(@delegate);

        container.AppendDelegate(new OpenTelemetryAsyncConvention((_, _, builder, token) => @delegate(builder, token)), priority, category);
        return container;
    }
}
