using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using App.Metrics;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Testing;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.Serilog;
using Serilog;
using Serilog.Extensions.Logging;
using ILogger = Serilog.ILogger;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Testing
{
    [PublicAPI]
    public class ConventionTestWebHost<TEntryPoint> : WebApplicationFactory<TEntryPoint>
        where TEntryPoint : class
    {
        private readonly List<Action<ConventionContextBuilder>> _hostBuilderActions = new List<Action<ConventionContextBuilder>>();

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

        public ConventionTestWebHost<TEntryPoint> ConfigureClock(int? unixTimeSeconds = null, Duration? advanceBy = null) => ConfigureClock(new FakeClock(Instant.FromUnixTimeSeconds(unixTimeSeconds ?? 1577836800), advanceBy ?? Duration.FromSeconds(1)));

        public ConventionTestWebHost<TEntryPoint> ConfigureClock(IClock clock) => ConfigureHostBuilder(
            builder =>
            {
                builder.PrependDelegate(
                    new ServiceConvention(
                        (context, configuration, services) =>
                        {
                            services.RemoveAll<IClock>();
                            services.AddSingleton(clock);
                        }
                    )
                );
            }
        );

        public ConventionTestWebHost<TEntryPoint> ConfigureLogger(ILogger logger)
            => ConfigureHostBuilder(builder =>
                {
                    builder.Set(logger);
                    var factory = new SerilogLoggerFactory(logger);
                    builder.Set<ILoggerFactory>(factory);
                    builder.Set(factory.CreateLogger(nameof(ConventionTestWebHost<object>)));
                }
            );

        public ConventionTestWebHost<TEntryPoint> ConfigureLoggerFactory(ILoggerFactory loggerFactory)
            => ConfigureHostBuilder(builder =>
                {
                    builder.Set(loggerFactory);
                    builder.Set(loggerFactory.CreateLogger(nameof(ConventionTestWebHost<object>)));
                }
            );

        public ConventionTestWebHost<TEntryPoint> ConfigureHostBuilder(Action<ConventionContextBuilder> action)
        {
            _hostBuilderActions.Add(action);
            return this;
        }
    }
}