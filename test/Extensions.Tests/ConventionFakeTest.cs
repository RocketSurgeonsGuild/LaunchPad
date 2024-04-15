using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Testing;
using Rocket.Surgery.Extensions.Testing;

namespace Extensions.Tests;

public abstract class ConventionFakeTest(ITestOutputHelper testOutputHelper) : AutoFakeTest(testOutputHelper)
{
    protected async Task Init(Action<ConventionContextBuilder>? action = null)
    {
        var conventionContextBuilder = ConventionContextBuilder.Create()
                                                               .ForTesting(DependencyContext.Load(GetType().Assembly)!, LoggerFactory)
                                                               .WithLogger(Logger);
        action?.Invoke(conventionContextBuilder);
        var context = await ConventionContext.FromAsync(conventionContextBuilder);

        Populate(await new ServiceCollection().ApplyConventionsAsync(context));
    }
}
