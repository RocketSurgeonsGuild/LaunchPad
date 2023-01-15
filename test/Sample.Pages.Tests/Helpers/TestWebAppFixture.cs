using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core.Domain;

namespace Sample.Pages.Tests.Helpers;

public class TestWebAppFixture : LaunchPadWebAppFixture<Startup>, IAsyncLifetime
{
    public TestWebAppFixture() : base(new SqliteExtension<RocketDbContext>())
    {
    }
}
