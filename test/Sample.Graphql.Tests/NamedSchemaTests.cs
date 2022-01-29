using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Testing;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Graphql.Tests;

public class NamedSchemaTests : LoggerTest, IAsyncLifetime
{
    private readonly IConventionContext _context;

    public NamedSchemaTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _context = ConventionContext.From(
            ConventionContextBuilder.Create()
                                    .ForTesting(DependencyContext.Load(GetType().Assembly), LoggerFactory)
                                    .WithLogger(LoggerFactory.CreateLogger(nameof(AutoFakeTest)))
                                    .ConfigureServices(
                                         (context, collection) =>
                                         {
                                             collection.AddGraphQL();
                                             collection.AddGraphQL("Named");
                                         }
                                     )
        );
        ExcludeSourceContext(nameof(NamedSchemaTests));
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
