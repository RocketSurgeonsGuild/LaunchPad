using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Testing;

namespace Extensions.Tests;

public abstract class ConventionFakeTest(ITestOutputHelper testOutputHelper) : AutoFakeTest<XUnitTestContext>(XUnitDefaults.CreateTestContext(testOutputHelper))
{
    protected async Task Init(Action<ConventionContextBuilder>? action = null)
    {
        var factory = CreateLoggerFactory();
        var conventionContextBuilder = ConventionContextBuilder
                                      .Create()
                                      .ForTesting(Imports.Instance, factory)
                                      .WithLogger(factory.CreateLogger("Test"));
        action?.Invoke(conventionContextBuilder);

        var context = await ConventionContext.FromAsync(conventionContextBuilder);
        var configuration = await new ConfigurationBuilder().ApplyConventionsAsync(context);
        context.Set<IConfiguration>(configuration.Build());

        Populate(await new ServiceCollection().ApplyConventionsAsync(context));
    }
}
