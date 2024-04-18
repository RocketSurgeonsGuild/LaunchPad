using Alba;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Testing;
using Rocket.Surgery.Hosting;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Testing;

/// <summary>
/// An <see cref="IAlbaExtension"/> that is used for launchpad <typeparamref name="TTestAssembly"/> is used to get the assembly to test
/// </summary>
/// <typeparam name="TTestAssembly"></typeparam>
public class LaunchPadExtension<TTestAssembly> : LaunchPadExtension
{
    /// <summary>
    /// Create the test extension
    /// </summary>
    /// <param name="loggerFactory"></param>
    public LaunchPadExtension(ILoggerFactory loggerFactory) : base(loggerFactory)
    {
    }
}

#pragma warning disable CA1816
#pragma warning disable CA1063
/// <summary>
/// An <see cref="IAlbaExtension"/> that is used for launchpad
/// </summary>
public class LaunchPadExtension(ILoggerFactory loggerFactory) : IAlbaExtension
{
    /// <inheritdoc />
    public virtual void Dispose()
    {
    }

    /// <inheritdoc />
    public virtual ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Virtual method that can be overridden to do something before the host is started
    /// </summary>
    /// <param name="host"></param>
    /// <returns></returns>
    public virtual Task Start(IAlbaHost host)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Virtual method that can be overridden to do something before the host is configured
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public virtual IHostBuilder Configure(IHostBuilder builder)
    {
        builder.ConfigureLogging((_, loggingBuilder) => loggingBuilder.Services.AddSingleton(loggerFactory));
        builder.ConfigureServices(s => s.AddSingleton<TestServer>(z => (TestServer)z.GetRequiredService<IServer>()));
        builder.ConfigureServices(s => s.AddHttpLogging(options => options.LoggingFields = HttpLoggingFields.All));

        return builder;
    }
}
#pragma warning restore CA1816
#pragma warning restore CA1063
