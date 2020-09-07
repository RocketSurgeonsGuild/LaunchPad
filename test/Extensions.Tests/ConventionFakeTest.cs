using System;
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
using Rocket.Surgery.Hosting;
using Xunit;
using Xunit.Abstractions;

namespace Extensions.Tests
{
    public abstract class ConventionFakeTest : AutoFakeTest
    {
        public ConventionFakeTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        protected void Init(Action<ConventionContextBuilder>? action = null)
        {
            var c = Container;
            var builder = TestHost.For(this, LoggerFactory)
               .WithLogger(Logger)
               .Create(
                    z =>
                    {
                        z.Set(HostType.UnitTest);
                        action?.Invoke(z);
                    });
            var conventions = builder.Conventions.GetAll(HostType.UnitTest);
            var services = builder.Parse();
            c.Populate(services);
            Rebuild(c);
        }
    }
}