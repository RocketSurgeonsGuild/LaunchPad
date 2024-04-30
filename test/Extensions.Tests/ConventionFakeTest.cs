using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Testing;
using Rocket.Surgery.Extensions.Testing;

namespace Extensions.Tests;

public abstract class ConventionFakeTest(ITestOutputHelper testOutputHelper) : AutoFakeTest(testOutputHelper)
{
    protected async Task Init(Action<ConventionContextBuilder>? action = null)
    {
        var conventionContextBuilder = ConventionContextBuilder
                                      .Create()
                                      .ForTesting(Imports.Instance, LoggerFactory)
                                      .WithLogger(Logger);
        action?.Invoke(conventionContextBuilder);

        var context = await ConventionContext.FromAsync(conventionContextBuilder);
        var configuration = await new ConfigurationBuilder().ApplyConventionsAsync(context);
        context.Set<IConfiguration>(configuration.Build());

        Populate(await new ServiceCollection().ApplyConventionsAsync(context));
    }
}