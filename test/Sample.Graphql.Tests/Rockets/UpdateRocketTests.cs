﻿using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Graphql.Tests.Helpers;
using CoreRocketType = Sample.Core.Domain.RocketType;

namespace Sample.Graphql.Tests.Rockets;

public class UpdateRocketTests(ITestContextAccessor testContext, GraphQlAppFixture rocketSurgeryWebAppFixture)
    : GraphQlWebAppFixtureTest<GraphQlAppFixture>(testContext, rocketSurgeryWebAppFixture)
{
    [Fact]
    public async Task Should_Update_A_Rocket()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();

        var rocket = await ServiceProvider
                          .WithScoped<RocketDbContext>()
                          .Invoke(
                               async z =>
                               {
                                   var rocket = new ReadyRocket
                                   {
                                       Type = CoreRocketType.Falcon9,
                                       SerialNumber = "12345678901234",
                                   };
                                   z.Add(rocket);

                                   await z.SaveChangesAsync();
                                   return rocket;
                               }
                           );

        var u = await client.UpdateRocket.ExecuteAsync(
            new()
            {
                Id = rocket.Id.Value,
                Type = RocketType.FalconHeavy,
                SerialNumber = string.Join("", rocket.SerialNumber.Reverse()),
            }
        );
        await Verify(u);
    }

    [Fact]
    public async Task Should_Patch_A_Rocket_SerialNumber()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();

        var rocket = await ServiceProvider
                          .WithScoped<RocketDbContext>()
                          .Invoke(
                               async z =>
                               {
                                   var rocket = new ReadyRocket
                                   {
                                       Type = CoreRocketType.AtlasV,
                                       SerialNumber = "12345678901234",
                                   };
                                   z.Add(rocket);

                                   await z.SaveChangesAsync();
                                   return rocket;
                               }
                           );

        var u = await client.PatchRocket.ExecuteAsync(
            new()
            {
                Id = rocket.Id.Value,
                SerialNumber = "123456789012345",
            }
        );
        u.EnsureNoErrors();

        await Verify(u);
    }

    [Fact]
    public async Task Should_Fail_To_Patch_A_Null_Rocket_SerialNumber()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();

        var rocket = await ServiceProvider
                          .WithScoped<RocketDbContext>()
                          .Invoke(
                               async z =>
                               {
                                   var rocket = new ReadyRocket
                                   {
                                       Type = CoreRocketType.AtlasV,
                                       SerialNumber = "12345678901234",
                                   };
                                   z.Add(rocket);

                                   await z.SaveChangesAsync();
                                   return rocket;
                               }
                           );


        var u = await client.PatchRocket.ExecuteAsync(
            new()
            {
                Id = rocket.Id.Value,
                SerialNumber = null,
            }
        );
        await Verify(u);
    }

    [Fact]
    public async Task Should_Patch_A_Rocket_Type()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();

        var rocket = await ServiceProvider
                          .WithScoped<RocketDbContext>()
                          .Invoke(
                               async z =>
                               {
                                   var rocket = new ReadyRocket
                                   {
                                       Type = CoreRocketType.AtlasV,
                                       SerialNumber = "12345678901234",
                                   };
                                   z.Add(rocket);

                                   await z.SaveChangesAsync();
                                   return rocket;
                               }
                           );

        var u = await client.PatchRocket.ExecuteAsync(
            new()
            {
                Id = rocket.Id.Value,
                Type = RocketType.FalconHeavy,
            }
        );

        await Verify(u);
    }

    [Theory]
    [ClassData(typeof(ShouldValidateUsersRequiredFieldData))]
    public async Task Should_Validate_Required_Fields(EditRocketRequest request, string propertyName)
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        var response = await client.UpdateRocket.ExecuteAsync(request);
        response.IsErrorResult().ShouldBeTrue();

        await Verify(response).UseParameters(request, propertyName).HashParameters();
    }

    private class ShouldValidateUsersRequiredFieldData : TheoryData<EditRocketRequest, string>
    {
        public ShouldValidateUsersRequiredFieldData()
        {
            var faker = new Faker
            {
                Random = new(1234567),
            };
            Add(
                new()
                {
                    Id = faker.Random.Guid(),
                    Type = RocketType.Falcon9,
                },
                nameof(EditRocketRequest.SerialNumber)
            );
            Add(
                new()
                {
                    Id = faker.Random.Guid(),
                    SerialNumber = faker.Random.String2(0, 9),
                    Type = RocketType.FalconHeavy,
                },
                nameof(EditRocketRequest.SerialNumber)
            );
            Add(
                new()
                {
                    Id = faker.Random.Guid(),
                    SerialNumber = faker.Random.String2(600, 800),
                    Type = RocketType.AtlasV,
                },
                nameof(EditRocketRequest.SerialNumber)
            );
            Add(
                new()
                {
                    Id = faker.Random.Guid(),
                    SerialNumber = faker.Random.String2(11),
                },
                nameof(EditRocketRequest.Type)
            );
        }
    }
}
