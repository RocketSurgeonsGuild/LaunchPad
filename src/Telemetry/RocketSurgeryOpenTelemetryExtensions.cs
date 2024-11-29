using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.Telemetry;

/// <summary>
///     Extension method to apply configuration conventions
/// </summary>
[PublicAPI]
public static class RocketSurgeryOpenTelemetryExtensions
{
    /// <summary>
    ///     Apply configuration conventions
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<IOpenTelemetryBuilder> ApplyConventionsAsync(
        this IOpenTelemetryBuilder builder,
        IConventionContext context,
        CancellationToken cancellationToken = default
    )
    {
        // If we don't get configuration, we're probably not needing telemetry
        if (context.Get<IConfiguration>() is not { } configuration) return builder;

        foreach (var item in context.Conventions
                                    .Get<IOpenTelemetryConvention, OpenTelemetryConvention, IOpenTelemetryAsyncConvention,
                                         OpenTelemetryAsyncConvention>())
        {
            switch (item)
            {
                case IOpenTelemetryConvention convention:
                    convention.Register(context, configuration, builder);
                    break;
                case OpenTelemetryConvention @delegate:
                    @delegate(context, configuration, builder);
                    break;
                case IOpenTelemetryAsyncConvention convention:
                    await convention.Register(context, configuration, builder, cancellationToken).ConfigureAwait(false);
                    break;
                case OpenTelemetryAsyncConvention @delegate:
                    await @delegate(context, configuration, builder, cancellationToken).ConfigureAwait(false);
                    break;
            }
        }

        return builder;
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
