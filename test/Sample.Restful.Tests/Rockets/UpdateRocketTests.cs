using Bogus;
using FluentAssertions;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Restful.Client;
using Xunit;
using Xunit.Abstractions;
using RocketType = Sample.Core.Domain.RocketType;

namespace Sample.Restful.Tests.Rockets;

public class UpdateRocketTests : HandleWebHostBase
{
    [Fact]
    public async Task Should_Update_A_Rocket()
    {
        var client = new RocketClient(Factory.CreateClient());

        var rocket = await ServiceProvider.WithScoped<RocketDbContext>()
                                          .Invoke(
                                               async z =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Type = RocketType.Falcon9,
                                                       SerialNumber = "12345678901234"
                                                   };
                                                   z.Add(rocket);

                                                   await z.SaveChangesAsync();
                                                   return rocket;
                                               }
                                           );

        var u = await client.EditRocketAsync(
            rocket.Id.Value,
            new EditRocketRequest
            {
                Type = Client.RocketType.FalconHeavy,
                SerialNumber = string.Join("", rocket.SerialNumber.Reverse())
            }
        );
        ( u.StatusCode is >= 200 and < 300 ).Should().BeTrue();

        // var response = await client.GetRocketAsync(rocket.Id);
        //
        // response.Type.Should().Be(RocketType.FalconHeavy);
        // response.Sn.Should().Be("43210987654321");
    }

    public UpdateRocketTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    private static readonly Faker Faker = new();

    [Theory]
    [ClassData(typeof(ShouldValidateUsersRequiredFieldData))]
    public async Task Should_Validate_Required_Fields(EditRocketRequest request, string propertyName)
    {
        var client = new RocketClient(Factory.CreateClient());
        Func<Task> a = () => client.EditRocketAsync(Guid.NewGuid(), request);
        ( await a.Should().ThrowAsync<ApiException<FluentValidationProblemDetails>>() )
           .And
           .Result.Errors.Values
           .SelectMany(x => x)
           .Select(z => z.PropertyName)
           .Should()
           .Contain(propertyName);
    }

    private class ShouldValidateUsersRequiredFieldData : TheoryData<EditRocketRequest, string>
    {
        public ShouldValidateUsersRequiredFieldData()
        {
            Add(
                new EditRocketRequest(),
                nameof(EditRocketRequest.SerialNumber)
            );
            Add(
                new EditRocketRequest { SerialNumber = Faker.Random.String2(0, 9) },
                nameof(EditRocketRequest.SerialNumber)
            );
            Add(
                new EditRocketRequest { SerialNumber = Faker.Random.String2(600, 800) },
                nameof(EditRocketRequest.SerialNumber)
            );
        }
    }
}
