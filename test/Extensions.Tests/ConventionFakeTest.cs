using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Conventions;

namespace Extensions.Tests;

public abstract class ConventionFakeTest(ITestContextAccessor testContext) : AutoFakeTest<XUnitTestContext>(XUnitDefaults.CreateTestContext(testContext))
{
    protected async Task Init(Action<ConventionContextBuilder>? action = null)
    {
        var conventionContextBuilder = ConventionContextBuilder
                                      .Create(Imports.Instance)
                                      .UseLogger(Logger);
        action?.Invoke(conventionContextBuilder);

        var context = await ConventionContext.FromAsync(conventionContextBuilder);
        var configuration = await new ConfigurationBuilder().ApplyConventionsAsync(context);
        context.Set<IConfiguration>(configuration.Build());

        Populate(await new ServiceCollection().ApplyConventionsAsync(context));
    }
}
