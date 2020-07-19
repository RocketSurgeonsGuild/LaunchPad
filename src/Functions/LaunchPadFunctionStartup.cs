using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.LaunchPad.Functions
{
    public abstract class LaunchPadFunctionStartup : FunctionsStartup
    {
        internal ServiceProviderDictionary Properties { get; } = new ServiceProviderDictionary();
        internal ILogger Logger;

        protected LaunchPadFunctionStartup()
        {
            var functionsAssembly = this.GetType().Assembly;

            var location = Path.GetDirectoryName(functionsAssembly.Location);
            DependencyContext? dependencyContext = null;
            while (!string.IsNullOrEmpty(location))
            {
                var depsFilePath = Path.Combine(location, functionsAssembly.GetName().Name + ".deps.json");
                if (File.Exists(depsFilePath))
                {
                    using var stream = File.Open(depsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    using var reader = new DependencyContextJsonReader();
                    dependencyContext = reader.Read(stream);
                    break;
                }

                location = Path.GetDirectoryName(location);
            }

            DiagnosticSource = new DiagnosticListener(nameof(LaunchPadFunctionStartup));
            Logger = new DiagnosticLogger(DiagnosticSource);
            AssemblyCandidateFinder = new DependencyContextAssemblyCandidateFinder(dependencyContext!, Logger);
            AssemblyProvider = new DependencyContextAssemblyProvider(dependencyContext!, Logger);
            Properties.Set(HostType.Live);
            Scanner = new SimpleConventionScanner(AssemblyCandidateFinder, Properties, Logger);
        }

        protected LaunchPadFunctionStartup(Action<LaunchPadFunctionStartup> configure) : this()
        {
            configure(this);
        }

        public sealed override void Configure(IFunctionsHostBuilder builder) => Compose(builder);
        public abstract void Setup(IFunctionsHostBuilder builder);

        public LaunchPadFunctionStartup UseRocketBooster(Action<LaunchPadFunctionStartup> configure)
        {
            configure(this);
            return this;
        }

        public LaunchPadFunctionStartup LaunchWith(Action<LaunchPadFunctionStartup> configure)
        {
            configure(this);
            return this;
        }


        /// <summary>Adds a set of conventions to the scanner</summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public LaunchPadFunctionStartup AppendConvention([NotNull] IEnumerable<IConvention> conventions)
        {
            Scanner.AppendConvention(conventions);
            return this;
        }

        /// <summary>Adds a set of conventions to the scanner</summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>LaunchPadFunctionStartup.</returns>
        public LaunchPadFunctionStartup AppendConvention(params IConvention[] conventions)
        {
            Scanner.AppendConvention(conventions);
            return this;
        }

        /// <summary>Adds a set of conventions to the scanner</summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>LaunchPadFunctionStartup.</returns>
        public LaunchPadFunctionStartup AppendConvention([NotNull] IEnumerable<Type> conventions)
        {
            Scanner.AppendConvention(conventions);
            return this;
        }

        /// <summary>Adds a set of conventions to the scanner</summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>LaunchPadFunctionStartup.</returns>
        public LaunchPadFunctionStartup AppendConvention(params Type[] conventions)
        {
            Scanner.AppendConvention(conventions);
            return this;
        }

        /// <summary>Adds a set of conventions to the scanner</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>LaunchPadFunctionStartup.</returns>
        public LaunchPadFunctionStartup AppendConvention<T>()
            where T : IConvention
        {
            Scanner.AppendConvention<T>();
            return this;
        }

        /// <summary>Adds a set of conventions to the scanner</summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>LaunchPadFunctionStartup.</returns>
        public LaunchPadFunctionStartup PrependConvention([NotNull] IEnumerable<IConvention> conventions)
        {
            Scanner.PrependConvention(conventions);
            return this;
        }

        /// <summary>Adds a set of conventions to the scanner</summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>LaunchPadFunctionStartup.</returns>
        public LaunchPadFunctionStartup PrependConvention(params IConvention[] conventions)
        {
            Scanner.PrependConvention(conventions);
            return this;
        }

        /// <summary>Adds a set of conventions to the scanner</summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>LaunchPadFunctionStartup.</returns>
        public LaunchPadFunctionStartup PrependConvention([NotNull] IEnumerable<Type> conventions)
        {
            Scanner.PrependConvention(conventions);
            return this;
        }

        /// <summary>Adds a set of conventions to the scanner</summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>LaunchPadFunctionStartup.</returns>
        public LaunchPadFunctionStartup PrependConvention(params Type[] conventions)
        {
            Scanner.PrependConvention(conventions);
            return this;
        }

        /// <summary>Adds a set of conventions to the scanner</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>LaunchPadFunctionStartup.</returns>
        public LaunchPadFunctionStartup PrependConvention<T>()
            where T : IConvention
        {
            Scanner.PrependConvention<T>();
            return this;
        }

        /// <summary>Adds a set of delegates to the scanner</summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>LaunchPadFunctionStartup.</returns>
        public LaunchPadFunctionStartup AppendDelegate([NotNull] IEnumerable<Delegate> delegates)
        {
            Scanner.AppendDelegate(delegates);
            return this;
        }

        /// <summary>Adds a set of delegates to the scanner</summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>LaunchPadFunctionStartup.</returns>
        public LaunchPadFunctionStartup AppendDelegate(params Delegate[] delegates)
        {
            Scanner.AppendDelegate(delegates);
            return this;
        }

        /// <summary>Adds a set of delegates to the scanner</summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>LaunchPadFunctionStartup.</returns>
        public LaunchPadFunctionStartup PrependDelegate([NotNull] IEnumerable<Delegate> delegates)
        {
            Scanner.PrependDelegate(delegates);
            return this;
        }

        /// <summary>Adds a set of delegates to the scanner</summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>LaunchPadFunctionStartup.</returns>
        public LaunchPadFunctionStartup PrependDelegate(params Delegate[] delegates)
        {
            Scanner.PrependDelegate(delegates);
            return this;
        }

        internal IConventionScanner Scanner
        {
            get => Properties.Get<IConventionScanner>();
            set
            {
                if (value == null)
                    return;
                Properties.Set(value);
            }
        }

        internal IAssemblyCandidateFinder AssemblyCandidateFinder
        {
            get => Properties.Get<IAssemblyCandidateFinder>();
            set
            {
                if (value == null)
                    return;
                Properties.Set(value);
            }
        }

        internal IAssemblyProvider AssemblyProvider
        {
            get => Properties.Get<IAssemblyProvider>();
            set
            {
                if (value == null)
                    return;
                Properties.Set(value);
            }
        }

        internal DiagnosticSource DiagnosticSource
        {
            get => Properties.Get<DiagnosticSource>();

            set
            {
                if (value == null)
                    return;
                Properties.Set(value);
                Logger = new DiagnosticLogger(value);
                Properties.Set(Logger);
            }
        }

        private void SetupServices(IConfiguration existingConfiguration, IServiceCollection services)
        {
            var hostEnvironment = services
               .Where(z => z.ServiceType == typeof(IHostEnvironment))
               .Select(x => x.ImplementationInstance)
               .OfType<IHostEnvironment>()
               .FirstOrDefault();
            if (hostEnvironment == null)
                throw new NotSupportedException("hostEnvironment could not be found");

            hostEnvironment.EnvironmentName = new[]
            {
                Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT"),
                Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT"),
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                Environment.GetEnvironmentVariable("WEBSITE_SLOT_NAME"),
                hostEnvironment.EnvironmentName
            }.First(z => !string.IsNullOrWhiteSpace(z));

            hostEnvironment.ApplicationName = new[]
            {
                Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"),
                hostEnvironment.ApplicationName
            }.First(z => !string.IsNullOrWhiteSpace(z));

            var builder = new ServicesBuilder(
                Scanner,
                AssemblyProvider,
                AssemblyCandidateFinder,
                services,
                existingConfiguration,
                hostEnvironment,
                Logger,
                Properties
            );

            Composer.Register<IServiceConventionContext, IServiceConvention, ServiceConventionDelegate>(
                Scanner.BuildProvider(),
                builder
            );
        }

        /// <summary>
        /// Composes this instance.
        /// </summary>
        private void Compose(IFunctionsHostBuilder builder)
        {
            var existingHostedServices = builder.Services.Where(x => x.ServiceType == typeof(IHostedService)).ToArray();
            if (this is IConvention convention)
            {
                Scanner.AppendConvention(convention);
            }


            var configuration = builder.Services
                   .Where(z => typeof(IConfiguration).IsAssignableFrom(z.ServiceType))
                   .Select(x => x.ImplementationInstance)
                   .OfType<IConfiguration>()
                   .FirstOrDefault() ??
                builder.Services
                   .Where(z => typeof(HostBuilderContext).IsAssignableFrom(z.ServiceType))
                   .Select(x => x.ImplementationInstance)
                   .OfType<HostBuilderContext>()
                   .FirstOrDefault()?.Configuration;
            if (configuration == null)
                throw new NotSupportedException($"Configuration could not be found, {string.Join("\n", builder.Services.Select(z => z.ServiceType.Name))}");

            SetupServices(configuration, builder.Services);

            builder.Services.RemoveAll<IHostedService>();
            builder.Services.Add(existingHostedServices);
        }
    }
}