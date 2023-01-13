using Alba;
using DryIoc;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc.Testing.Handlers;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Testing;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting;
using Sample.Core.Domain;
using Serilog.Events;
using CoreRocketType = Sample.Core.Domain.RocketType;

namespace Sample.Graphql.Tests.Rockets;

internal class FixtureLoggerFactory : ILoggerFactory
{
    private ILoggerFactory? _innerLoggerFactory;

    public void SetLoggerFactory(ILoggerFactory loggerFactory)
    {
        _innerLoggerFactory = loggerFactory;
    }

    public void Dispose()
    {
        _innerLoggerFactory?.Dispose();
    }

    public void AddProvider(ILoggerProvider provider)
    {
        _innerLoggerFactory?.AddProvider(provider);
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _innerLoggerFactory?.CreateLogger(categoryName) ?? NullLogger.Instance;
    }
}

public class AppFixture : IAsyncLifetime
{
    private readonly FixtureLoggerFactory _loggerFactory;
    private readonly SqliteExtension _sqlExtension;

    public AppFixture()
    {
        _loggerFactory = new FixtureLoggerFactory();
        _sqlExtension = new SqliteExtension();
    }

    public IAlbaHost AlbaHost { get; private set; } = null!;

    public void SetLoggerFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory.SetLoggerFactory(loggerFactory);
    }

    public void Reset()
    {
        _sqlExtension.Reset();
    }

    public Task ResetAsync()
    {
        return _sqlExtension.ResetAsync();
    }

    public async Task InitializeAsync()
    {
        AlbaHost = await Alba.AlbaHost.For<Program>(new RocketSurgeryExtension(_loggerFactory), new LocalExtension(), _sqlExtension);
    }

    public async Task DisposeAsync()
    {
        await AlbaHost.DisposeAsync();
    }
}

public class CreateRocketTests : LoggerTest, IClassFixture<AppFixture>, IAsyncLifetime

{
    [Fact]
    public async Task Should_Create_A_Rocket()
    {
        var client = _host.Services.GetRequiredService<IRocketClient>();
        var response = await client.CreateRocket.ExecuteAsync(
            new CreateRocketRequest
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            }
        );

        response.EnsureNoErrors();
        response.Data!.CreateRocket.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_Throw_If_Rocket_Exists()
    {
        var client = _host.Services.GetRequiredService<IRocketClient>();
        var response = await client.CreateRocket.ExecuteAsync(
            new CreateRocketRequest
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            }
        );
        response.IsErrorResult().Should().BeFalse();

        var response2 = await client.CreateRocket.ExecuteAsync(
            new CreateRocketRequest
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            }
        );
        response2.IsErrorResult().Should().BeTrue();
        response2.Errors[0].Message.Should().Be("A Rocket already exists with that serial number!");
    }

    public CreateRocketTests(ITestOutputHelper testOutputHelper, AppFixture appFixture) : base(testOutputHelper, LogEventLevel.Information)
    {
        _appFixture = appFixture;
        _host = appFixture.AlbaHost;
    }

    private readonly AppFixture _appFixture;
    private readonly IAlbaHost _host;

    public Task InitializeAsync()
    {
        return _appFixture.ResetAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
