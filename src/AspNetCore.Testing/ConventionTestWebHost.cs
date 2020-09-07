using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;
using Serilog;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Testing
{
    [PublicAPI]
    public class ConventionTestWebHost<TEntryPoint> : WebApplicationFactory<TEntryPoint>
        where TEntryPoint : class
    {
        private readonly List<Action<IHostBuilder>> _hostBuilderActions = new List<Action<IHostBuilder>>();

        protected override IHostBuilder CreateHostBuilder()
        {
            var hostBuilder = base.CreateHostBuilder()
               .UseContentRoot(Path.GetDirectoryName(typeof(TEntryPoint).Assembly.Location))
               .ConfigureRocketSurgery(
                    builder => builder.Set(HostType.UnitTest)
                );
            foreach (var item in _hostBuilderActions)
            {
                item(hostBuilder);
            }

            return hostBuilder;
        }

        public ConventionTestWebHost<TEntryPoint> ConfigureLoggerFactory(ILoggerFactory loggerFactory) => ConfigureHostBuilder(
            builder =>
            {
                builder.ConfigureRocketSurgery(
                    c =>
                    {
                        c.ConfigureHosting(
                            b =>
                            {
                               b.UseSerilog((context, logger) => logger.ApplyConventions()))
                            });
                        );
                    }
                );
            });

        public ConventionTestWebHost<TEntryPoint> ConfigureHostBuilder(Action<IHostBuilder> action)
        {
            _hostBuilderActions.Add(action);
            return this;
        }
    }
}