using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Testing;
using Rocket.Surgery.Extensions.Testing;

namespace Extensions.Tests;

public abstract class ConventionFakeTest(ITestOutputHelper testOutputHelper) : AutoFakeTest(testOutputHelper)
{
    protected void Init(Action<ConventionContextBuilder>? action = null)
    {
        var conventionContextBuilder = ConventionContextBuilder.Create()
                                                               .ForTesting(DependencyContext.Load(GetType().Assembly)!, LoggerFactory)
                                                               .WithLogger(Logger);
        action?.Invoke(conventionContextBuilder);
        var context = ConventionContext.From(conventionContextBuilder);

        Populate(new ServiceCollection().ApplyConventions(context));
    }
}
