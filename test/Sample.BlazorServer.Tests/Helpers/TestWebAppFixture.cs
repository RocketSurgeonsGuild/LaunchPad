using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core.Domain;

namespace Sample.BlazorServer.Tests.Helpers;

public class TestWebAppFixture : LaunchPadWebAppFixture<Startup>, IAsyncLifetime
{
    public TestWebAppFixture() : base(new SqliteExtension<RocketDbContext>())
    {
    }
}
