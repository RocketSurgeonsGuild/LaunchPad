﻿using Grpc.Core;
using Sample.Grpc.Tests.Helpers;
using Sample.Grpc.Tests.Validation;
using R = Sample.Grpc.Rockets;

namespace Sample.Grpc.Tests.Rockets;

public class CreateRocketTests(ITestContextAccessor testContext, TestWebAppFixture webAppFixture)
    : WebAppFixtureTest<TestWebAppFixture>(testContext, webAppFixture)
{
    [Fact]
    public async Task Should_Create_A_Rocket()
    {
        var client = new R.RocketsClient(AlbaHost.CreateGrpcChannel());

        var response = await client.CreateRocketAsync(
            new CreateRocketRequest
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            }
        );

        response.Id.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task Should_Throw_If_Rocket_Exists()
    {
        var client = new R.RocketsClient(AlbaHost.CreateGrpcChannel());
        await client.CreateRocketAsync(
            new CreateRocketRequest
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            }
        );

        Func<Task> action = async () => await client.CreateRocketAsync(
            new CreateRocketRequest
            {
                Type = RocketType.Falcon9,
                SerialNumber = "12345678901234"
            }
        );
        var r = (await action.ShouldThrowAsync<RpcException>());
        r.Message.ShouldContain("Rocket Creation Failed");
    }
}
