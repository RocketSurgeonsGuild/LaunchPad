using Rocket.Surgery.LaunchPad.AspNetCore.Testing;

using Sample.Core.Domain;

using TUnit.Core.Interfaces;

namespace Sample.BlazorServer.Tests.Helpers;

public class TestWebAppFixture() : LaunchPadWebAppFixture<Program>(new SqliteExtension<RocketDbContext>()), IAsyncInitializer;
