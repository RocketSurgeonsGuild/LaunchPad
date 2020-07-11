using System.Threading.Tasks;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.TestHost;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Extensions.Tests
{
    public abstract class ConventionFakeTest : AutoFakeTest, IAsyncLifetime
    {
        protected ConventionTestHost HostBuilder { get; }

        public ConventionFakeTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            HostBuilder = ConventionTestHostBuilder.For(this, LoggerFactory)
               .With(Logger)
               .With(DiagnosticSource)
               .Create();
        }

        protected virtual void Build(IConventionHostBuilder builder) { }

        public async Task InitializeAsync()
        {
            await Task.Yield();
            Build(HostBuilder);
            Populate(HostBuilder.Parse());
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}