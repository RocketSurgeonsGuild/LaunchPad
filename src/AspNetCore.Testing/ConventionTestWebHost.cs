using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using App.Metrics;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.Serilog;
using Serilog;

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
                        foreach (var item in _hostBuilderActions)
                        {
                            item(builder);
                        }
                    }
                );

            return hostBuilder;
        }

        public ConventionTestWebHost<TEntryPoint> ConfigureLoggerFactory(ILoggerFactory loggerFactory)
            => ConfigureHostBuilder(builder => builder.Set(loggerFactory));

        public ConventionTestWebHost<TEntryPoint> ConfigureHostBuilder(Action<ConventionContextBuilder> action)
        {
            _hostBuilderActions.Add(action);
            return this;
        }
    }
}