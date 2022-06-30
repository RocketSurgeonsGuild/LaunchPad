using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using NodaTime;
using NodaTime.Testing;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Testing;
using Rocket.Surgery.Hosting;
using Serilog;
using Serilog.Extensions.Logging;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Testing;

[ExportConvention]
public class HostingConvention : IHostingConvention, IServiceConvention
{
    public void Register(IConventionContext context, IHostBuilder builder)
    {
    }

    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
    }
}

internal sealed class HostingListener : IObserver<DiagnosticListener>, IObserver<KeyValuePair<string, object?>>, IDisposable
{
    private static readonly AsyncLocal<HostingListener> _currentListener = new();
    private readonly List<Action<ConventionContextBuilder>> _hostBuilderActions = new();
    private readonly CompositeDisposable _disposable = new();

    public void Attach()
    {
        _currentListener.Value = this;
        _disposable.Add(DiagnosticListener.AllListeners.Subscribe(this));
    }

    /// <summary>
    ///     Add additional configuration to the Host
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public HostingListener ConfigureHostBuilder(Action<ConventionContextBuilder> action)
    {
        _hostBuilderActions.Add(action);
        return this;
    }

    public void Dispose()
    {
        _disposable.Dispose();
    }

    public void OnCompleted()
    {
        _disposable.Dispose();
    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(DiagnosticListener value)
    {
        if (_currentListener.Value != this)
        {
            // Ignore events that aren't for this listener
            return;
        }

        if (value.Name == "Microsoft.Extensions.Hosting")
        {
            _disposable.Add(value.Subscribe(this));
        }
    }

    public void OnNext(KeyValuePair<string, object?> value)
    {
        if (_currentListener.Value != this)
        {
            // Ignore events that aren't for this listener
            return;
        }

        if (value.Key == "HostBuilding")
        {
            var builder = (IHostBuilder)value.Value!;
            builder.ConfigureRocketSurgery(
                z =>
                {
                    z.Set(HostType.UnitTest);
                    z.IncludeConvention(typeof(HostingListener).Assembly);
                    foreach (var item in _hostBuilderActions)
                    {
                        item(z);
                    }
                }
            );
        }
    }
}

/// <summary>
///     A base test class that configures rocket surgery for unit or integration testing
/// </summary>
/// <typeparam name="TEntryPoint"></typeparam>
[PublicAPI]
public class ConventionTestWebHost<TEntryPoint> : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    private HostingListener _listener = new HostingListener();

    public ConventionTestWebHost()
    {
        ConfigureClock();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        _listener.Attach();
        return base.CreateHost(builder);
    }

    /// <summary>
    ///     Configure the default <see cref="FakeClock" />
    /// </summary>
    /// <param name="unixTimeSeconds"></param>
    /// <param name="advanceBy"></param>
    /// <returns></returns>
    public ConventionTestWebHost<TEntryPoint> ConfigureClock(int? unixTimeSeconds = null, Duration? advanceBy = null)
    {
        return ConfigureClock(new FakeClock(Instant.FromUnixTimeSeconds(unixTimeSeconds ?? 1577836800), advanceBy ?? Duration.FromSeconds(1)));
    }

    /// <summary>
    ///     Configure the default <see cref="IClock" />
    /// </summary>
    /// <param name="clock"></param>
    /// <returns></returns>
    public ConventionTestWebHost<TEntryPoint> ConfigureClock(IClock clock)
    {
        return ConfigureHostBuilder(
            builder =>
            {
                builder.PrependDelegate(
                    new ServiceConvention(
                        (_, _, services) =>
                        {
                            services.RemoveAll<IClock>();
                            services.AddSingleton(clock);
                        }
                    )
                );
            }
        );
    }

    /// <summary>
    ///     Configure the the default <see cref="ILogger" />
    /// </summary>
    /// <param name="logger"></param>
    /// <returns></returns>
    public ConventionTestWebHost<TEntryPoint> ConfigureLogger(ILogger logger)
    {
        return ConfigureHostBuilder(
            builder =>
            {
                builder.Set(logger);
                var factory = new SerilogLoggerFactory(logger);
                builder.Set<ILoggerFactory>(factory);
                builder.Set(factory.CreateLogger(nameof(ConventionTestWebHost<object>)));
            }
        );
    }

    /// <summary>
    ///     Configure the the default <see cref="ILoggerFactory" />
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <returns></returns>
    public ConventionTestWebHost<TEntryPoint> ConfigureLoggerFactory(ILoggerFactory loggerFactory)
    {
        return ConfigureHostBuilder(
            builder =>
            {
                builder.Set(loggerFactory);
                builder.Set(loggerFactory.CreateLogger(nameof(ConventionTestWebHost<object>)));
            }
        );
    }

    /// <summary>
    ///     Add additional configuration to the Host
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public ConventionTestWebHost<TEntryPoint> ConfigureHostBuilder(Action<ConventionContextBuilder> action)
    {
        _listener.ConfigureHostBuilder(action);
        return this;
    }
}
