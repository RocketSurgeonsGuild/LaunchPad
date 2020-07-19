using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Extensions.Tests
{
    public abstract class ConventionFakeTest : AutoFakeTest, IAsyncLifetime
    {
        protected TestHostBuilder HostBuilder { get; }

        public ConventionFakeTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            HostBuilder = TestHost.For(this, LoggerFactory)
               .WithLogger(Logger)
               .Create();
        }

        protected override IContainer BuildContainer(IContainer container)
        {
            var services = HostBuilder.Parse();

            var builder = new ServicesBuilder(
                HostBuilder.Scanner,
                HostBuilder.AssemblyProvider,
                HostBuilder.AssemblyCandidateFinder,
                services,
                new ConfigurationBuilder().Build(),
                services.Select(z => z.ImplementationInstance).OfType<IHostEnvironment>().First(),
                HostBuilder.Get<ILogger>(),
                HostBuilder.ServiceProperties
            );
            Composer.Register(HostBuilder.Scanner, builder, typeof(IServiceConvention), typeof(ServiceConventionDelegate));
            container.Populate(services);

            return container;
        }

        public async Task InitializeAsync()
        {
            await Task.Yield();
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}