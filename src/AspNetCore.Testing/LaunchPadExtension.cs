using System.Reflection;
using Alba;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Testing;
using Rocket.Surgery.Hosting;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Testing;

public class LaunchPadExtension<TTestAssembly> : LaunchPadExtension
{
    public LaunchPadExtension(ILoggerFactory loggerFactory) : base(typeof(TTestAssembly), loggerFactory)
    {
    }
}

public class LaunchPadExtension : IAlbaExtension
{
    private readonly Assembly _testAssemblyReference;
    private readonly ILoggerFactory _loggerFactory;

    public LaunchPadExtension(object testAssemblyReference, ILoggerFactory loggerFactory) : base()
    {
        _testAssemblyReference = testAssemblyReference.GetType().Assembly;
        _loggerFactory = loggerFactory;
    }

    public LaunchPadExtension(Type testAssemblyReference, ILoggerFactory loggerFactory)
    {
        _testAssemblyReference = testAssemblyReference.Assembly;
        _loggerFactory = loggerFactory;
    }

    public void Dispose()
    {
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public Task Start(IAlbaHost host)
    {
        return Task.CompletedTask;
    }

    public IHostBuilder Configure(IHostBuilder builder)
    {
        builder.ConfigureRocketSurgery(z => z.ForTesting(_testAssemblyReference, _loggerFactory));
        builder.ConfigureLogging((context, loggingBuilder) => loggingBuilder.Services.AddSingleton(_loggerFactory));
        builder.ConfigureServices(s => s.AddSingleton<TestServer>(z => (TestServer)z.GetRequiredService<IServer>()));
        builder.ConfigureServices(s => s.AddHttpLogging(options => options.LoggingFields = HttpLoggingFields.All));

        return builder;
    }
}
