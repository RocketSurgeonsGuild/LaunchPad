using DryIoc;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.DependencyInjection;
using Rocket.Surgery.LaunchPad.AspNetCore.Testing;
using Sample.Core.Domain;
using Sample.Graphql.Tests.Helpers;
using CoreRocketType = Sample.Core.Domain.RocketType;

namespace Sample.Graphql.Tests.Rockets;

public class UpdateRocketTests : GraphQlWebAppFixtureTest<GraphQlAppFixture>
{
    private static readonly Faker Faker = new();

    public UpdateRocketTests(ITestOutputHelper testOutputHelper, GraphQlAppFixture rocketSurgeryWebAppFixture) : base(testOutputHelper, rocketSurgeryWebAppFixture)
    {
    }

    [Fact]
    public async Task Should_Update_A_Rocket()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();

        var rocket = await ServiceProvider.WithScoped<RocketDbContext>()
                                          .Invoke(
                                               async z =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Type = CoreRocketType.Falcon9,
                                                       SerialNumber = "12345678901234"
                                                   };
                                                   z.Add(rocket);

                                                   await z.SaveChangesAsync();
                                                   return rocket;
                                               }
                                           );

        var u = await client.UpdateRocket.ExecuteAsync(
            new EditRocketRequest
            {
                Id = rocket.Id.Value,
                Type = RocketType.FalconHeavy,
                SerialNumber = string.Join("", rocket.SerialNumber.Reverse())
            }
        );
        u.IsSuccessResult().Should().Be(true);
    }

    [Fact]
    public async Task Should_Patch_A_Rocket_SerialNumber()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();

        var rocket = await ServiceProvider.WithScoped<RocketDbContext>()
                                          .Invoke(
                                               async z =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Type = CoreRocketType.AtlasV,
                                                       SerialNumber = "12345678901234"
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
                SerialNumber = "123456789012345"
            }
        );
        u.EnsureNoErrors();

        u.Data!.PatchRocket.Type.Should().Be(RocketType.AtlasV);
        u.Data!.PatchRocket.SerialNumber.Should().Be("123456789012345");
    }

    [Fact]
    public async Task Should_Fail_To_Patch_A_Null_Rocket_SerialNumber()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();

        var rocket = await ServiceProvider.WithScoped<RocketDbContext>()
                                          .Invoke(
                                               async z =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Type = CoreRocketType.AtlasV,
                                                       SerialNumber = "12345678901234"
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
                SerialNumber = null
            }
        );

        u.IsErrorResult().Should().BeTrue();
        u.Errors[0].Message.Should().Be("'Serial Number' must not be empty.");
    }

    [Fact]
    public async Task Should_Patch_A_Rocket_Type()
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();

        var rocket = await ServiceProvider.WithScoped<RocketDbContext>()
                                          .Invoke(
                                               async z =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Type = CoreRocketType.AtlasV,
                                                       SerialNumber = "12345678901234"
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
                Type = RocketType.FalconHeavy
            }
        );

        u.Data!.PatchRocket.Type.Should().Be(RocketType.FalconHeavy);
        u.Data!.PatchRocket.SerialNumber.Should().Be("12345678901234");
    }

    [Theory]
    [ClassData(typeof(ShouldValidateUsersRequiredFieldData))]
    public async Task Should_Validate_Required_Fields(EditRocketRequest request, string propertyName)
    {
        var client = ServiceProvider.GetRequiredService<IRocketClient>();
        request = request with { Id = Guid.NewGuid() };
        var response = await client.UpdateRocket.ExecuteAsync(request);
        response.IsErrorResult().Should().BeTrue();
        response.Errors[0].Extensions!["field"].As<string>().Split('.').Last()
                .Pascalize().Should().Be(propertyName);
    }

    private class ShouldValidateUsersRequiredFieldData : TheoryData<EditRocketRequest, string>
    {
        public ShouldValidateUsersRequiredFieldData()
        {
            Add(
                new EditRocketRequest { Type = RocketType.Falcon9 },
                nameof(EditRocketRequest.SerialNumber)
            );
            Add(
                new EditRocketRequest { SerialNumber = Faker.Random.String2(0, 9), Type = RocketType.FalconHeavy },
                nameof(EditRocketRequest.SerialNumber)
            );
            Add(
                new EditRocketRequest { SerialNumber = Faker.Random.String2(600, 800), Type = RocketType.AtlasV },
                nameof(EditRocketRequest.SerialNumber)
            );
            Add(
                new EditRocketRequest { SerialNumber = Faker.Random.String2(11) },
                nameof(EditRocketRequest.Type)
            );
        }
    }
}
