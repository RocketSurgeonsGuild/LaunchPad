using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using CoreRocketType = Sample.Core.Domain.RocketType;

namespace Sample.Graphql.Tests.Rockets;

public class UpdateRocketTests : HandleWebHostBase
{
    [Fact]
    public async Task Should_Update_A_Rocket()
    {
        var client = Factory.Services.GetRequiredService<IRocketClient>();

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

    public UpdateRocketTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    private static readonly Faker Faker = new();

    [Theory]
    [ClassData(typeof(ShouldValidateUsersRequiredFieldData))]
    public async Task Should_Validate_Required_Fields(EditRocketRequest request, string propertyName)
    {
        var client = Factory.Services.GetRequiredService<IRocketClient>();
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
