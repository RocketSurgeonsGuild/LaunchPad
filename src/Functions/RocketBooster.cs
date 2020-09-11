using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

//using Microsoft.Azure.WebJobs.Hosting;

namespace Rocket.Surgery.LaunchPad.Functions
{
    /// <summary>
    /// Class RocketBooster.
    /// </summary>
    public static class RocketBooster
    {
        /// <summary>
        /// Fors the application domain.
        /// </summary>
        /// <param name="appDomain">The application domain.</param>
        /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
        public static Func<LaunchPadFunctionStartup, ConventionContextBuilder> ForAppDomain(AppDomain appDomain)
            => builder => new ConventionContextBuilder(new Dictionary<object, object?>()).UseAppDomain(appDomain);

        /// <summary>
        /// Fors the specified application domain.
        /// </summary>
        /// <param name="appDomain">The application domain.</param>
        /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
        public static Func<LaunchPadFunctionStartup, ConventionContextBuilder> For(AppDomain appDomain) => ForAppDomain(appDomain);

        /// <summary>
        /// Fors the assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
        public static Func<LaunchPadFunctionStartup, ConventionContextBuilder> ForAssemblies(IEnumerable<Assembly> assemblies)
            => builder => new ConventionContextBuilder(new Dictionary<object, object?>()).UseAssemblies(assemblies);

        /// <summary>
        /// Fors the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
        public static Func<LaunchPadFunctionStartup, ConventionContextBuilder> For(IEnumerable<Assembly> assemblies) => ForAssemblies(assemblies);

        /// <summary>
        /// Applys all conventions for hosting, configuration, services and logging
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="builder"></param>
        /// <param name="contextBuilder"></param>
        public static IConventionContext ApplyConventions(LaunchPadFunctionStartup startup, IFunctionsHostBuilder builder, ConventionContextBuilder contextBuilder)
        {
            if (startup is IConvention convention)
            {
                contextBuilder.AppendConvention(convention);
            }
            return ApplyConventions(builder, ConventionContext.From(contextBuilder));
        }

        /// <summary>
        /// Applys all conventions for hosting, configuration, services and logging
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="context"></param>
        public static IConventionContext ApplyConventions(IFunctionsHostBuilder builder, IConventionContext context)
        {
            var existingHostedServices = builder.Services.Where(x => x.ServiceType == typeof(IHostedService)).ToArray();

            var hostContext = builder.Services
               .Where(z => typeof(HostBuilderContext).IsAssignableFrom(z.ServiceType))
               .Select(x => x.ImplementationInstance)
               .OfType<HostBuilderContext>()
               .FirstOrDefault();
            if (hostContext == null)
                throw new NotSupportedException("hostContext could not be found");

            context.Set(hostContext.Configuration);
            context.Set(hostContext.HostingEnvironment);

            var hostEnvironment = hostContext.HostingEnvironment;

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

            builder.Services
               .ApplyConventions(context)
               .AddLogging(z => z.ApplyConventions(context));

            builder.Services.RemoveAll<IHostedService>();
            builder.Services.Add(existingHostedServices);
            return context;
        }

        /// <summary>
        /// Uses the assembly candidate finder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="logger">The assembly candidate finder.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static LaunchPadFunctionStartup UseDiagnosticLogger(
            [NotNull] this LaunchPadFunctionStartup builder,
            [NotNull] ILogger logger
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            builder._builder.Set(logger);
            return builder;
        }

        /// <summary>
        /// Uses the diagnostic logging.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="action">The action.</param>
        /// <returns>LaunchPadFunctionStartup.</returns>
        public static LaunchPadFunctionStartup UseDiagnosticLogging(
            [NotNull] this LaunchPadFunctionStartup builder,
            [NotNull] Action<ILoggingBuilder> action
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            builder.UseDiagnosticLogger(
                new ServiceCollection()
                   .AddLogging(action)
                   .BuildServiceProvider()
                   .GetRequiredService<ILoggerFactory>()
                   .CreateLogger("DiagnosticLogger")
            );

            return builder;
        }
    }
}