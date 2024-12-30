using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.Conventions;

namespace Extensions.Tests;

public abstract class ConventionFakeTest(ITestOutputHelper testOutputHelper) : AutoFakeTest<XUnitTestContext>(XUnitDefaults.CreateTestContext(testOutputHelper))
{
    protected async Task Init(Action<ConventionContextBuilder>? action = null)
    {
        var conventionContextBuilder = ConventionContextBuilder
                                      .Create(Imports.Instance);
        action?.Invoke(conventionContextBuilder);

        var context = await ConventionContext.FromAsync(conventionContextBuilder);
        var configuration = await new ConfigurationBuilder().ApplyConventionsAsync(context);
        _ = context.Set<IConfiguration>(configuration.Build());

        Populate(await new ServiceCollection().ApplyConventionsAsync(context));
    }
}
