using Alba;
using Microsoft.AspNetCore.Mvc.Testing;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core.Domain;

namespace Sample.Grpc.Tests.Helpers;

public class TestWebAppFixture() : LaunchPadWebAppFixture<Program>(new SqliteExtension<RocketDbContext>()), IAsyncLifetime;
