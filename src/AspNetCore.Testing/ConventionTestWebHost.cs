using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;

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
               .ConfigureRocketSurgery(
                    builder => { builder.Set(HostType.UnitTestHost); }
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
                builder.ConfigureServices(
                    services =>
                    {
                        services.AddSingleton(loggerFactory);
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