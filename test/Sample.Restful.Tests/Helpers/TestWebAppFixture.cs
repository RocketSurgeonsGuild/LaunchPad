using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core.Domain;

namespace Sample.Restful.Tests.Helpers;

public class TestWebAppFixture() : LaunchPadWebAppFixture<Program>(new SqliteExtension<RocketDbContext>()), IAsyncLifetime;
