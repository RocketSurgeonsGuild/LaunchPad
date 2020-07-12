using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;

//using Microsoft.Azure.WebJobs.Hosting;

namespace Rocket.Surgery.LaunchPad.Functions
{
    /// <summary>
    /// Class RocketBooster.
    /// </summary>
    public static class RocketBooster
    {
        /// <summary>
        /// Configures using a dependency context
        /// </summary>
        /// <param name="startup">The startup instance.</param>
        /// <param name="dependencyContext">The dependency context.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>Func&lt;IWebJobsBuilder, System.Object, IFunctionsHostBuilder&gt;.</returns>
        public static LaunchPadFunctionStartup ConfigureForDependencyContext(
            this LaunchPadFunctionStartup startup,
            DependencyContext dependencyContext,
            DiagnosticSource? diagnosticSource = null
        )
        {
            ForDependencyContext(dependencyContext, diagnosticSource)(startup);
            return startup;
        }

        /// <summary>
        /// Fors the dependency context.
        /// </summary>
        /// <param name="dependencyContext">The dependency context.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>Func&lt;IWebJobsBuilder, System.Object, IFunctionsHostBuilder&gt;.</returns>
        public static Action<LaunchPadFunctionStartup> ForDependencyContext(
            DependencyContext dependencyContext,
            DiagnosticSource? diagnosticSource = null
        ) => (builder) =>
        {
            if (diagnosticSource != null)
                builder.DiagnosticSource = diagnosticSource;
            var assemblyCandidateFinder = new DependencyContextAssemblyCandidateFinder(dependencyContext, builder.Logger);
            var assemblyProvider = new DependencyContextAssemblyProvider(dependencyContext, builder.Logger);
            var scanner = new SimpleConventionScanner(assemblyCandidateFinder, builder.Properties, builder.Logger);
            builder.Scanner = scanner;
            builder.AssemblyProvider = assemblyProvider;
            builder.AssemblyCandidateFinder = assemblyCandidateFinder;
        };

        /// <summary>
        /// Fors the specified dependency context.
        /// </summary>
        /// <param name="dependencyContext">The dependency context.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>Func&lt;IWebJobsBuilder, System.Object, IFunctionsHostBuilder&gt;.</returns>
        public static Action<LaunchPadFunctionStartup> For(
            DependencyContext dependencyContext,
            DiagnosticSource? diagnosticSource = null
        ) => ForDependencyContext(dependencyContext, diagnosticSource);

        /// <summary>
        /// Configures using an app domain
        /// </summary>
        /// <param name="startup">The startup instance.</param>
        /// <param name="appDomain">The application domain.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>Func&lt;IWebJobsBuilder, System.Object, IFunctionsHostBuilder&gt;.</returns>
        public static LaunchPadFunctionStartup ConfigureForAppDomain(
            this LaunchPadFunctionStartup startup,
            AppDomain appDomain,
            DiagnosticSource? diagnosticSource = null
        )
        {
            ForAppDomain(appDomain, diagnosticSource)(startup);
            return startup;
        }

        /// <summary>
        /// Fors the application domain.
        /// </summary>
        /// <param name="appDomain">The application domain.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>Func&lt;IWebJobsBuilder, System.Object, IFunctionsHostBuilder&gt;.</returns>
        public static Action<LaunchPadFunctionStartup> ForAppDomain(
            AppDomain appDomain,
            DiagnosticSource? diagnosticSource = null
        ) => (builder) =>
        {
            if (diagnosticSource != null)
                builder.DiagnosticSource = diagnosticSource;

            var assemblyCandidateFinder = new AppDomainAssemblyCandidateFinder(appDomain, builder.Logger);
            var assemblyProvider = new AppDomainAssemblyProvider(appDomain, builder.Logger);
            var scanner = new SimpleConventionScanner(assemblyCandidateFinder, builder.Properties, builder.Logger);

            builder.Scanner = scanner;
            builder.AssemblyProvider = assemblyProvider;
            builder.AssemblyCandidateFinder = assemblyCandidateFinder;
        };

        /// <summary>
        /// Fors the specified application domain.
        /// </summary>
        /// <param name="appDomain">The application domain.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>Func&lt;IWebJobsBuilder, System.Object, IFunctionsHostBuilder&gt;.</returns>
        public static Action<LaunchPadFunctionStartup> For(
            AppDomain appDomain,
            DiagnosticSource? diagnosticSource = null
        ) => ForAppDomain(appDomain, diagnosticSource);

        /// <summary>
        /// Configures using an app domain
        /// </summary>
        /// <param name="startup">The startup instance.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>Func&lt;IWebJobsBuilder, System.Object, IFunctionsHostBuilder&gt;.</returns>
        public static LaunchPadFunctionStartup ConfigureForAssemblies(
            this LaunchPadFunctionStartup startup,
            IEnumerable<Assembly> assemblies,
            DiagnosticSource? diagnosticSource = null
        )
        {
            ForAssemblies(assemblies, diagnosticSource)(startup);
            return startup;
        }

        /// <summary>
        /// Fors the assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>Func&lt;IWebJobsBuilder, System.Object, IFunctionsHostBuilder&gt;.</returns>
        public static Action<LaunchPadFunctionStartup> ForAssemblies(
            IEnumerable<Assembly> assemblies,
            DiagnosticSource? diagnosticSource = null
        ) => (builder) =>
        {
            if (diagnosticSource != null)
                builder.DiagnosticSource = diagnosticSource;

            var enumerable = assemblies as Assembly[] ?? assemblies.ToArray();
            var assemblyCandidateFinder = new DefaultAssemblyCandidateFinder(enumerable, builder.Logger);
            var assemblyProvider = new DefaultAssemblyProvider(enumerable, builder.Logger);
            var scanner = new SimpleConventionScanner(assemblyCandidateFinder, builder.Properties, builder.Logger);

            builder.Scanner = scanner;
            builder.AssemblyProvider = assemblyProvider;
            builder.AssemblyCandidateFinder = assemblyCandidateFinder;
        };

        /// <summary>
        /// Fors the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>Func&lt;IWebJobsBuilder, System.Object, IFunctionsHostBuilder&gt;.</returns>
        public static Action<LaunchPadFunctionStartup> For(
            IEnumerable<Assembly> assemblies,
            DiagnosticSource? diagnosticSource = null
        ) => ForAssemblies(assemblies, diagnosticSource);


        /// <summary>
        /// Uses the scanner.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="scanner">The scanner.</param>
        /// <returns>LaunchPadFunctionStartup.</returns>
        public static LaunchPadFunctionStartup UseScanner(
            [NotNull] this LaunchPadFunctionStartup builder,
            [NotNull] IConventionScanner scanner
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (scanner == null)
            {
                throw new ArgumentNullException(nameof(scanner));
            }

            builder.Scanner = scanner;
            return builder;
        }

        /// <summary>
        /// Uses the assembly candidate finder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <returns>LaunchPadFunctionStartup.</returns>
        public static LaunchPadFunctionStartup UseAssemblyCandidateFinder(
            [NotNull] this LaunchPadFunctionStartup builder,
            [NotNull] IAssemblyCandidateFinder assemblyCandidateFinder
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (assemblyCandidateFinder == null)
            {
                throw new ArgumentNullException(nameof(assemblyCandidateFinder));
            }

            builder.AssemblyCandidateFinder = assemblyCandidateFinder;
            return builder;
        }

        /// <summary>
        /// Uses the assembly provider.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <returns>LaunchPadFunctionStartup.</returns>
        public static LaunchPadFunctionStartup UseAssemblyProvider(
            [NotNull] this LaunchPadFunctionStartup builder,
            [NotNull] IAssemblyProvider assemblyProvider
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (assemblyProvider == null)
            {
                throw new ArgumentNullException(nameof(assemblyProvider));
            }

            builder.AssemblyProvider = assemblyProvider;
            return builder;
        }

        /// <summary>
        /// Uses the diagnostic source.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>LaunchPadFunctionStartup.</returns>
        public static LaunchPadFunctionStartup UseDiagnosticSource(
            [NotNull] this LaunchPadFunctionStartup builder,
            [NotNull] DiagnosticSource diagnosticSource
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (diagnosticSource == null)
            {
                throw new ArgumentNullException(nameof(diagnosticSource));
            }


            builder.DiagnosticSource = diagnosticSource;
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

            ( builder.DiagnosticSource is DiagnosticListener listener
                ? listener
                : new DiagnosticListener("DiagnosticLogger") ).SubscribeWithAdapter(
                new DiagnosticListenerLoggingAdapter(
                    new ServiceCollection()
                       .AddLogging(action)
                       .BuildServiceProvider()
                       .GetRequiredService<ILoggerFactory>()
                       .CreateLogger("DiagnosticLogger")
                )
            );
            return builder;
        }
    }
}