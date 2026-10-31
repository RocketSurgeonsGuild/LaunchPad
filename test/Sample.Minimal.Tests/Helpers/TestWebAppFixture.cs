using Rocket.Surgery.LaunchPad.AspNetCore.Testing;

using Sample.Core.Domain;

using TUnit.Core.Interfaces;

namespace Sample.Minimal.Tests.Helpers;

public class TestWebAppFixture() : LaunchPadWebAppFixture<Program>(new SqliteExtension<RocketDbContext>()), IAsyncInitializer;
