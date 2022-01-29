using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;

//using Microsoft.Azure.WebJobs.Hosting;

namespace Rocket.Surgery.LaunchPad.Functions;

/// <summary>
///     Class RocketBooster.
/// </summary>
public static class RocketBooster
{
    /// <summary>
    ///     Fors the application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <param name="getConventions"></param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<LaunchPadFunctionStartup, ConventionContextBuilder> ForAppDomain(
        AppDomain appDomain,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions
    )
    {
        return _ => new ConventionContextBuilder(new Dictionary<object, object?>())
                   .UseAppDomain(appDomain)
                   .WithConventionsFrom(getConventions);
    }

    /// <summary>
    ///     Fors the application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <param name="conventionContextBuilderAction"></param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<LaunchPadFunctionStartup, ConventionContextBuilder> ForAppDomain(
        AppDomain appDomain,
        Action<ConventionContextBuilder>? conventionContextBuilderAction = null
    )
    {
        return _ =>
        {
            var conventionContextBuilder = new ConventionContextBuilder(new Dictionary<object, object?>())
               .UseAppDomain(appDomain);
            conventionContextBuilderAction?.Invoke(conventionContextBuilder);
            return conventionContextBuilder;
        };
    }

    /// <summary>
    ///     Fors the specified application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <param name="getConventions"></param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<LaunchPadFunctionStartup, ConventionContextBuilder> For(
        AppDomain appDomain,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions
    )
    {
        return ForAppDomain(appDomain, getConventions);
    }

    /// <summary>
    ///     Fors the specified application domain.
    /// </summary>
    /// <param name="appDomain">The application domain.</param>
    /// <param name="conventionContextBuilderAction"></param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<LaunchPadFunctionStartup, ConventionContextBuilder> For(
        AppDomain appDomain,
        Action<ConventionContextBuilder>? conventionContextBuilderAction = null
    )
    {
        return ForAppDomain(appDomain, conventionContextBuilderAction);
    }

    /// <summary>
    ///     Fors the assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="getConventions"></param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<LaunchPadFunctionStartup, ConventionContextBuilder> ForAssemblies(
        IEnumerable<Assembly> assemblies,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions
    )
    {
        return _ => new ConventionContextBuilder(new Dictionary<object, object?>())
                   .UseAssemblies(assemblies)
                   .WithConventionsFrom(getConventions);
    }

    /// <summary>
    ///     Fors the assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="conventionContextBuilderAction"></param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<LaunchPadFunctionStartup, ConventionContextBuilder> ForAssemblies(
        IEnumerable<Assembly> assemblies,
        Action<ConventionContextBuilder>? conventionContextBuilderAction = null
    )
    {
        return _ =>
        {
            var conventionContextBuilder = new ConventionContextBuilder(new Dictionary<object, object?>()).UseAssemblies(assemblies);
            conventionContextBuilderAction?.Invoke(conventionContextBuilder);
            return conventionContextBuilder;
        };
    }

    /// <summary>
    ///     Fors the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="getConventions"></param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<LaunchPadFunctionStartup, ConventionContextBuilder> For(
        IEnumerable<Assembly> assemblies,
        Func<IServiceProvider, IEnumerable<IConventionWithDependencies>> getConventions
    )
    {
        return ForAssemblies(assemblies, getConventions);
    }

    /// <summary>
    ///     Fors the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <param name="conventionContextBuilderAction"></param>
    /// <returns>Func&lt;IHostBuilder, ConventionContextBuilder&gt;.</returns>
    public static Func<LaunchPadFunctionStartup, ConventionContextBuilder> For(
        IEnumerable<Assembly> assemblies,
        Action<ConventionContextBuilder>? conventionContextBuilderAction = null
    )
    {
        return ForAssemblies(assemblies, conventionContextBuilderAction);
    }

    // /// <summary>
    // /// Use the given dependency context for resolving assemblies
    // /// </summary>
    // /// <param name="dependencyContext"></param>
    // /// <returns></returns>
    // public static Func<LaunchPadFunctionStartup, ConventionContextBuilder> ForDependencyContext(DependencyContext dependencyContext)
    //     => builder => new ConventionContextBuilder(new Dictionary<object, object?>()).UseDependencyContext(dependencyContext);
    //
    // /// <summary>
    // /// Use the given dependency context for resolving assemblies
    // /// </summary>
    // /// <param name="dependencyContext"></param>
    // /// <returns></returns>
    // public static Func<LaunchPadFunctionStartup, ConventionContextBuilder> For(DependencyContext dependencyContext) => ForDependencyContext(dependencyContext);

    // /// <summary>
    // /// Use the given dependency context for resolving assemblies
    // /// </summary>
    // /// <param name="dependencyContext"></param>
    // /// <returns></returns>
    // public static Func<LaunchPadFunctionStartup, ConventionContextBuilder> ForDependencyContext(DependencyContext dependencyContext)
    //     => builder => new ConventionContextBuilder(new Dictionary<object, object?>()).UseDependencyContext(dependencyContext);
    //
    // /// <summary>
    // /// Use the given dependency context for resolving assemblies
    // /// </summary>
    // /// <param name="dependencyContext"></param>
    // /// <returns></returns>
    // public static Func<LaunchPadFunctionStartup, ConventionContextBuilder> For(DependencyContext dependencyContext) => ForDependencyContext(dependencyContext);

    /// <summary>
    ///     Uses the assembly candidate finder.
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
    ///     Uses the diagnostic logging.
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
