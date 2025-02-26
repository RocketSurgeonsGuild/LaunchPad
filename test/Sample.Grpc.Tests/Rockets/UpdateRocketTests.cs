using Grpc.Core;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Grpc.Tests.Helpers;
using Sample.Grpc.Tests.Validation;
using R = Sample.Grpc.Rockets;

namespace Sample.Grpc.Tests.Rockets;

public class UpdateRocketTests(ITestContextAccessor testContext, TestWebAppFixture testWebAppFixture)
    : WebAppFixtureTest<TestWebAppFixture>(testContext, testWebAppFixture)
{
    private static readonly Faker Faker = new();

    [Fact]
    public async Task Should_Update_A_Rocket()
    {
        var client = new R.RocketsClient(AlbaHost.CreateGrpcChannel());

        var rocket = await ServiceProvider.WithScoped<RocketDbContext>()
                                          .Invoke(
                                               async z =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Type = Core.Domain.RocketType.Falcon9,
                                                       SerialNumber = "12345678901234"
                                                   };
                                                   z.Add(rocket);

                                                   await z.SaveChangesAsync();
                                                   return rocket;
                                               }
                                           );

        var u = await client.EditRocketAsync(
            new UpdateRocketRequest
            {
                Id = rocket.Id.ToString(),
                Type = RocketType.FalconHeavy,
                SerialNumber = string.Join("", rocket.SerialNumber.Reverse())
            }
        );
        u.ShouldNotBeNull();

        // var response = await client.GetRocketAsync(rocket.Id);
        //
        // response.Type.ShouldBe(RocketType.FalconHeavy);
        // response.Sn.ShouldBe("43210987654321");
    }

    [Theory]
    [ClassData(typeof(ShouldValidateUsersRequiredFieldData))]
    public async Task Should_Validate_Required_Fields(UpdateRocketRequest request, string propertyName)
    {
        var client = new R.RocketsClient(AlbaHost.CreateGrpcChannel());
        Func<Task> a = async () => await client.EditRocketAsync(request);
        var e = (await a.ShouldThrowAsync<RpcException>());
        e.Status.StatusCode.ShouldBe(StatusCode.InvalidArgument);
        e.Message.ShouldContain(propertyName);
    }

    private sealed class ShouldValidateUsersRequiredFieldData : TheoryData<UpdateRocketRequest, string>
    {
        public ShouldValidateUsersRequiredFieldData()
        {
            Add(
                new UpdateRocketRequest(),
                nameof(UpdateRocketRequest.SerialNumber)
            );
            Add(
                new UpdateRocketRequest { SerialNumber = Faker.Random.String2(0, 9) },
                nameof(UpdateRocketRequest.SerialNumber)
            );
            Add(
                new UpdateRocketRequest { SerialNumber = Faker.Random.String2(600, 800) },
                nameof(UpdateRocketRequest.SerialNumber)
            );
        }
    }
}
