using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NodaTime;
using NodaTime.Testing;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Hosting;
using Serilog;
using Serilog.Extensions.Logging;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Testing;

/// <summary>
///     A base test class that configures rocket surgery for unit or integration testing
/// </summary>
/// <typeparam name="TEntryPoint"></typeparam>
[PublicAPI]
public class ConventionTestWebHost<TEntryPoint> : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    private readonly List<Action<ConventionContextBuilder>> _hostBuilderActions = new List<Action<ConventionContextBuilder>>();

    /// <inheritdoc />
    protected override IHostBuilder CreateHostBuilder()
    {
        var hostBuilder = base.CreateHostBuilder()
                              .UseContentRoot(Path.GetDirectoryName(typeof(TEntryPoint).Assembly.Location))
                              .ConfigureRocketSurgery(
                                   builder =>
                                   {
                                       builder.Set(HostType.UnitTest);
                                       ConfigureClock();
                                       foreach (var item in _hostBuilderActions)
                                       {
                                           item(builder);
                                       }
                                   }
                               );

        return hostBuilder;
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
        _hostBuilderActions.Add(action);
        return this;
    }
}
