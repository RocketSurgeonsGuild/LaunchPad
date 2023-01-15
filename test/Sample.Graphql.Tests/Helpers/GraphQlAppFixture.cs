using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core.Domain;

namespace Sample.Graphql.Tests.Helpers;

public class GraphQlAppFixture : LaunchPadWebAppFixture<Program>, IAsyncLifetime
{
    public GraphQlAppFixture() : base(new SqliteExtension<RocketDbContext>(), new GraphQlExtension())
    {
    }
}
