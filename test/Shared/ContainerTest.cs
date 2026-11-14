using DryIoc;
using DryIoc.Microsoft.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog.Extensions.Logging;

namespace Rocket.Surgery.LaunchPad.Testing;

/// <summary>
///     A test with a DryIoc container whose loggers write to the test log, for tests that resolve services from a populated service collection.
/// </summary>
/// <remarks>
///     It takes the place of <c>AutoFakeTest</c> from <c>Rocket.Surgery.Extensions.Testing.FakeItEasy</c> without the automatic fakes: a test that
///     needs a mock creates it with TUnit.Mocks and registers it. The file is linked into each test project that uses it.
/// </remarks>
public abstract class ContainerTest<TContext>(TContext context) : LoggerTest<TContext>(context)
    where TContext : class, ILoggingTestContext
{

    /// <summary>
    ///     The DryIoc container.
    /// </summary>
    protected IContainer Container => Provider.Container;

    /// <summary>
    ///     The service provider over <see cref="Container" />, built on first use.
    /// </summary>
    protected IServiceProvider ServiceProvider => Provider;

    private DryIocServiceProvider Provider { get => field ??= Build(new Container()); set; }

    /// <summary>
    ///     Adds the services to the container.
    /// </summary>
    protected void Populate(IServiceCollection services) => Container.Populate(services);

    /// <summary>
    ///     Lets a test change the container before the test services are added to it.
    /// </summary>
    protected virtual IContainer BuildContainer(IContainer container) => container;

    private DryIocServiceProvider Build(IContainer container)
    {
        container = container.With(
            rules => rules
                    .WithTestLoggerResolver((request, loggerType) => ActivatorUtilities.CreateInstance(request.Container, loggerType))
                    .WithConcreteTypeDynamicRegistrations((_, _) => true, Reuse.Transient)
        );
        container.RegisterDelegate(
            resolver => CreateLoggerFactory(resolver.Resolve<LoggerProviderCollection>(IfUnresolved.ReturnDefault) ?? new LoggerProviderCollection())
        );
        container.RegisterDelegate(resolver => resolver.Resolve<ILoggerFactory>().CreateLogger("Test"));
        container.RegisterInstance(Logger);
        return BuildContainer(container).WithDependencyInjectionAdapter();
    }
}
