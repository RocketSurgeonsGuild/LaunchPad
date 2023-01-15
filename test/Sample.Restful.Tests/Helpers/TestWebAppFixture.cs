using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.BlazorServer.Tests;
using Sample.Core.Domain;

namespace Sample.Restful.Tests.Helpers;

public class TestWebAppFixture : LaunchPadWebAppFixture<Program>, IAsyncLifetime
{
    public TestWebAppFixture() : base(new SqliteExtension<RocketDbContext>())
    {
    }
}
