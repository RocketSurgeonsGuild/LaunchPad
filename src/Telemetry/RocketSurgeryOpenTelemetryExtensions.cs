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
    /// <param name="conventionContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<IOpenTelemetryBuilder> ApplyConventionsAsync(
        this IOpenTelemetryBuilder builder,
        IConventionContext conventionContext,
        CancellationToken cancellationToken = default
    )
    {
        // If we don't get configuration, we're probably not needing telemetry
        if (conventionContext.Get<IConfiguration>() is not { } configuration) return builder;

        foreach (var item in conventionContext.Conventions
                                              .Get<IOpenTelemetryConvention, OpenTelemetryConvention, IOpenTelemetryAsyncConvention,
                                                   OpenTelemetryAsyncConvention>())
        {
            switch (item)
            {
                case IOpenTelemetryConvention convention:
                    convention.Register(conventionContext, configuration, builder);
                    break;
                case OpenTelemetryConvention @delegate:
                    @delegate(conventionContext, configuration, builder);
                    break;
                case IOpenTelemetryAsyncConvention convention:
                    await convention.Register(conventionContext, configuration, builder, cancellationToken);
                    break;
                case OpenTelemetryAsyncConvention @delegate:
                    await @delegate(conventionContext, configuration, builder, cancellationToken);
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
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureOpenTelemetry(this ConventionContextBuilder container, OpenTelemetryConvention @delegate)
    {
        ArgumentNullException.ThrowIfNull(container);

        ArgumentNullException.ThrowIfNull(@delegate);

        container.AppendDelegate(@delegate);
        return container;
    }


    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureOpenTelemetry(this ConventionContextBuilder container, Action<IOpenTelemetryBuilder> @delegate)
    {
        ArgumentNullException.ThrowIfNull(container);

        ArgumentNullException.ThrowIfNull(@delegate);

        container.AppendDelegate(@delegate);
        return container;
    }

    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureOpenTelemetry(this ConventionContextBuilder container, OpenTelemetryAsyncConvention @delegate)
    {
        ArgumentNullException.ThrowIfNull(container);

        ArgumentNullException.ThrowIfNull(@delegate);

        container.AppendDelegate(@delegate);
        return container;
    }

    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureOpenTelemetry(this ConventionContextBuilder container, Func<IOpenTelemetryBuilder, ValueTask> @delegate)
    {
        ArgumentNullException.ThrowIfNull(container);

        ArgumentNullException.ThrowIfNull(@delegate);

        container.AppendDelegate(new OpenTelemetryAsyncConvention((_, _, builder, _) => @delegate(builder)));
        return container;
    }

    /// <summary>
    ///     Configure the serilog delegate to the convention scanner
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="delegate">The delegate.</param>
    /// <returns>IConventionHostBuilder.</returns>
    public static ConventionContextBuilder ConfigureOpenTelemetry(
        this ConventionContextBuilder container,
        Func<IOpenTelemetryBuilder, CancellationToken, ValueTask> @delegate
    )
    {
        ArgumentNullException.ThrowIfNull(container);

        ArgumentNullException.ThrowIfNull(@delegate);

        container.AppendDelegate(new OpenTelemetryAsyncConvention((_, _, builder, token) => @delegate(builder, token)));
        return container;
    }
}