using App.Metrics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Serilog;
using Serilog.Extensions.Logging;
using ILogger = Serilog.ILogger;

namespace Rocket.Surgery.LaunchPad.Functions
{
    public abstract class LaunchPadFunctionStartup : FunctionsStartup
    {
        internal ConventionContextBuilder _builder;
        internal IConventionContext _context;

        protected LaunchPadFunctionStartup()
        {
            _builder = new ConventionContextBuilder(new Dictionary<object, object?>())
               .UseAppDomain(AppDomain.CurrentDomain)
               .Set(HostType.Live);
            if (this is IConvention convention)
            {
                _builder.AppendConvention(convention);
            }

            // TODO: Restore this sometime
            // var functionsAssembly = this.GetType().Assembly;
            // var location = Path.GetDirectoryName(functionsAssembly.Location);
            // DependencyContext? dependencyContext = null;
            // while (!string.IsNullOrEmpty(location))
            // {
            //     var depsFilePath = Path.Combine(location, functionsAssembly.GetName().Name + ".deps.json");
            //     if (File.Exists(depsFilePath))
            //     {
            //         using var stream = File.Open(depsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            //         using var reader = new DependencyContextJsonReader();
            //         dependencyContext = reader.Read(stream);
            //         break;
            //     }
            //
            //     location = Path.GetDirectoryName(location);
            // }
        }

        protected LaunchPadFunctionStartup(Func<LaunchPadFunctionStartup, ConventionContextBuilder> configure) : this()
        {
            _builder = configure(this).Set(HostType.Live);
            if (this is IConvention convention)
            {
                _builder.AppendConvention(convention);
            }
        }

        public sealed override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            Setup(_builder);
            _context = ConventionContext.From(_builder);
            builder.ConfigurationBuilder.ApplyConventions(_context);
            ConfigureAppConfiguration(builder, _context);
        }

        public sealed override void Configure(IFunctionsHostBuilder builder)
        {
            var existingHostedServices = builder.Services.Where(x => x.ServiceType == typeof(IHostedService)).ToArray();
            var builderContext = builder.GetContext();

            _context.Set(builderContext.Configuration);
            builder.Services.ApplyConventions(_context);

            builder.Services.RemoveAll<IHostedService>();
            builder.Services.Add(existingHostedServices);
            Configure(builder, _context);
        }

        public virtual void Setup(ConventionContextBuilder contextBuilder) { }

        public virtual void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder, IConventionContext context) { }

        public abstract void Configure(IFunctionsHostBuilder builder, IConventionContext context);

        public LaunchPadFunctionStartup UseRocketBooster(Func<LaunchPadFunctionStartup, ConventionContextBuilder> configure)
        {
            _builder = configure(this);
            return this;
        }

        public LaunchPadFunctionStartup LaunchWith(Func<LaunchPadFunctionStartup, ConventionContextBuilder> configure)
        {
            _builder = configure(this);
            return this;
        }
    }
}