using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Rocket.Surgery.LaunchPad.Hosting;

/// <summary>
///     Extensions for configuring the application lifecycle
/// </summary>
[PublicAPI]
public static class ApplicationLifecycleExtensions
{
    /// <summary>
    ///     Run a simple action when the host has started
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T OnHostStarted<T>(this T builder, Action<IServiceProvider> action) where T : IHostApplicationBuilder
    {
        builder.Services.AddSingleton(
            new ApplicationLifecycleRegistration(
                nameof(IHostedLifecycleService.StartedAsync),
                (provider, _) =>
                {
                    action(provider);
                    return Task.CompletedTask;
                }
            )
        );
        return builder;
    }

    /// <summary>
    ///     Run a simple action when the host has started
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T OnHostStarted<T>(this T builder, Func<IServiceProvider, Task> action) where T : IHostApplicationBuilder
    {
        builder.Services.AddSingleton(new ApplicationLifecycleRegistration(nameof(IHostedLifecycleService.StartedAsync), (provider, _) => action(provider)));
        return builder;
    }

    /// <summary>
    ///     Run a simple action when the host has started
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T OnHostStarted<T>(this T builder, Func<IServiceProvider, CancellationToken, Task> action) where T : IHostApplicationBuilder
    {
        builder.Services.AddSingleton(new ApplicationLifecycleRegistration(nameof(IHostedLifecycleService.StartedAsync), action));
        return builder;
    }

    /// <summary>
    ///     Run a simple action when the host has starting
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T OnHostStarting<T>(this T builder, Action<IServiceProvider> action) where T : IHostApplicationBuilder
    {
        builder.Services.AddSingleton(
            new ApplicationLifecycleRegistration(
                nameof(IHostedLifecycleService.StartingAsync),
                (provider, _) =>
                {
                    action(provider);
                    return Task.CompletedTask;
                }
            )
        );
        return builder;
    }

    /// <summary>
    ///     Run a simple action when the host has starting
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T OnHostStarting<T>(this T builder, Func<IServiceProvider, Task> action) where T : IHostApplicationBuilder
    {
        builder.Services.AddSingleton(new ApplicationLifecycleRegistration(nameof(IHostedLifecycleService.StartingAsync), (provider, _) => action(provider)));
        return builder;
    }

    /// <summary>
    ///     Run a simple action when the host has starting
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T OnHostStarting<T>(this T builder, Func<IServiceProvider, CancellationToken, Task> action) where T : IHostApplicationBuilder
    {
        builder.Services.AddSingleton(new ApplicationLifecycleRegistration(nameof(IHostedLifecycleService.StartingAsync), action));
        return builder;
    }

    /// <summary>
    ///     Run a simple action when the host has stopping
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T OnHostStopping<T>(this T builder, Action<IServiceProvider> action) where T : IHostApplicationBuilder
    {
        builder.Services.AddSingleton(
            new ApplicationLifecycleRegistration(
                nameof(IHostedLifecycleService.StoppingAsync),
                (provider, _) =>
                {
                    action(provider);
                    return Task.CompletedTask;
                }
            )
        );
        return builder;
    }

    /// <summary>
    ///     Run a simple action when the host has stopping
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T OnHostStopping<T>(this T builder, Func<IServiceProvider, Task> action) where T : IHostApplicationBuilder
    {
        builder.Services.AddSingleton(new ApplicationLifecycleRegistration(nameof(IHostedLifecycleService.StoppingAsync), (provider, _) => action(provider)));
        return builder;
    }

    /// <summary>
    ///     Run a simple action when the host has stopping
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T OnHostStopping<T>(this T builder, Func<IServiceProvider, CancellationToken, Task> action) where T : IHostApplicationBuilder
    {
        builder.Services.AddSingleton(new ApplicationLifecycleRegistration(nameof(IHostedLifecycleService.StoppingAsync), action));
        return builder;
    }

    /// <summary>
    ///     Run a simple action when the host has stopped
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T OnHostStopped<T>(this T builder, Action<IServiceProvider> action) where T : IHostApplicationBuilder
    {
        builder.Services.AddSingleton(
            new ApplicationLifecycleRegistration(
                nameof(IHostedLifecycleService.StoppedAsync),
                (provider, _) =>
                {
                    action(provider);
                    return Task.CompletedTask;
                }
            )
        );
        return builder;
    }

    /// <summary>
    ///     Run a simple action when the host has stopped
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T OnHostStopped<T>(this T builder, Func<IServiceProvider, Task> action) where T : IHostApplicationBuilder
    {
        builder.Services.AddSingleton(new ApplicationLifecycleRegistration(nameof(IHostedLifecycleService.StoppedAsync), (provider, _) => action(provider)));
        return builder;
    }

    /// <summary>
    ///     Run a simple action when the host has stopped
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T OnHostStopped<T>(this T builder, Func<IServiceProvider, CancellationToken, Task> action) where T : IHostApplicationBuilder
    {
        builder.Services.AddSingleton(new ApplicationLifecycleRegistration(nameof(IHostedLifecycleService.StoppedAsync), action));
        return builder;
    }
}