using Alba;

using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Testing;

/// <summary>
///     An <see cref="IAlbaExtension" /> that is used for launchpad <typeparamref name="TTestAssembly" /> is used to get the assembly to test
/// </summary>
/// <typeparam name="TTestAssembly"></typeparam>
/// <remarks>
///     Create the test extension
/// </remarks>
/// <param name="loggerFactory"></param>
public class LaunchPadExtension<TTestAssembly>(ILoggerFactory loggerFactory) : LaunchPadExtension(loggerFactory)
{
}

#pragma warning disable CA1816, CA1063
/// <summary>
///     An <see cref="IAlbaExtension" /> that is used for launchpad
/// </summary>
public class LaunchPadExtension(ILoggerFactory loggerFactory) : IAlbaExtension
{
    /// <inheritdoc />
    public virtual void Dispose() { }

    /// <inheritdoc />
    public virtual ValueTask DisposeAsync() => ValueTask.CompletedTask;

    /// <summary>
    ///     Virtual method that can be overridden to do something before the host is started
    /// </summary>
    /// <param name="host"></param>
    /// <returns></returns>
    public virtual Task Start(IAlbaHost host) => Task.CompletedTask;

    /// <summary>
    ///     Virtual method that can be overridden to do something before the host is configured
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public virtual IHostBuilder Configure(IHostBuilder builder)
    {
        builder.ConfigureLogging((_, loggingBuilder) => loggingBuilder.Services.AddSingleton(loggerFactory));
        builder.ConfigureServices(s => s.AddSingleton(z => (TestServer)z.GetRequiredService<IServer>()));
        builder.ConfigureServices(s => s.AddHttpLogging(options => options.LoggingFields = HttpLoggingFields.All));

        return builder;
    }
}
#pragma warning restore CA1816, CA1063
