using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core.Domain;

namespace Sample.Classic.Restful.Tests.Helpers;

public class TestWebAppFixture : LaunchPadWebAppFixture<Startup>, IAsyncLifetime
{

    public TestWebAppFixture() : base(new SqliteExtension<RocketDbContext>())
    {
    }
}
