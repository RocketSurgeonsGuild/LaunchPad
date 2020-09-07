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

namespace Rocket.Surgery.LaunchPad.Functions
{
    public abstract class LaunchPadFunctionStartup : FunctionsStartup
    {
        internal ConventionContextBuilder _builder;
        internal ILogger Logger;

        protected LaunchPadFunctionStartup()
        {
            _builder = new ConventionContextBuilder(new Dictionary<object, object?>())
               .UseAppDomain(AppDomain.CurrentDomain)
               .Set(HostType.Live);

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
        }

        public sealed override void Configure(IFunctionsHostBuilder builder)
        {
            Setup(builder, _builder);
            RocketBooster.ApplyConventions(this, builder, _builder);
        }

        public abstract void Setup(IFunctionsHostBuilder builder, ConventionContextBuilder contextBuilder);

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