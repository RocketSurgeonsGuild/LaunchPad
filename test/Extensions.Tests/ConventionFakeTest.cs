using DryIoc.Microsoft.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Rocket.Surgery.Conventions.Testing;
using Xunit.Abstractions;

namespace Extensions.Tests
{
    public abstract class ConventionFakeTest : AutoFakeTest
    {
        public ConventionFakeTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        protected void Init(Action<ConventionContextBuilder>? action = null)
        {
            var conventionContextBuilder = ConventionContextBuilder.Create()
                                                                   .ForTesting(DependencyContext.Load(GetType().Assembly), LoggerFactory)
                                                                   .WithLogger(Logger);
            action?.Invoke(conventionContextBuilder);
            var context = ConventionContext.From(conventionContextBuilder);

            Populate(new ServiceCollection().ApplyConventions(context));
        }
    }
}
