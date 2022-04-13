using Bogus;
using FluentAssertions;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Restful.Client;
using Xunit;
using Xunit.Abstractions;
using RocketType = Sample.Core.Domain.RocketType;
using ClientRocketType = Sample.Restful.Client.RocketType;

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
                Type = ClientRocketType.FalconHeavy,
                SerialNumber = string.Join("", rocket.SerialNumber.Reverse())
            }
        );
        ( u.StatusCode is >= 200 and < 300 ).Should().BeTrue();

        var response = await client.GetRocketAsync(rocket.Id.Value);

        response.Result.Type.Should().Be(ClientRocketType.FalconHeavy);
        response.Result.Sn.Should().Be("43210987654321");
    }

    [Fact]
    public async Task Should_Patch_A_Rocket_SerialNumber()
    {
        var client = new RocketClient(Factory.CreateClient());

        var rocket = await ServiceProvider.WithScoped<RocketDbContext>()
                                          .Invoke(
                                               async z =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Type = RocketType.AtlasV,
                                                       SerialNumber = "12345678901234"
                                                   };
                                                   z.Add(rocket);

                                                   await z.SaveChangesAsync();
                                                   return rocket;
                                               }
                                           );

        var u = await client.PatchRocketAsync(
            rocket.Id.Value,
            new EditRocketPatchRequest
            {
                SerialNumber = new() { Value = "123456789012345" }
            }
        );
        ( u.StatusCode is >= 200 and < 300 ).Should().BeTrue();

        var response = await client.GetRocketAsync(rocket.Id.Value);

        response.Result.Type.Should().Be(ClientRocketType.AtlasV);
        response.Result.Sn.Should().Be("123456789012345");
    }

    [Fact]
    public async Task Should_Fail_To_Patch_A_Null_Rocket_SerialNumber()
    {
        var client = new RocketClient(Factory.CreateClient());

        var rocket = await ServiceProvider.WithScoped<RocketDbContext>()
                                          .Invoke(
                                               async z =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Type = RocketType.AtlasV,
                                                       SerialNumber = "12345678901234"
                                                   };
                                                   z.Add(rocket);

                                                   await z.SaveChangesAsync();
                                                   return rocket;
                                               }
                                           );

        Func<Task<Response<RocketModel>>> action = () => client.PatchRocketAsync(
            rocket.Id.Value,
            new EditRocketPatchRequest
            {
                SerialNumber = new() { Value = null }
            }
        );
        var result = ( await action.Should().ThrowAsync<ApiException<FluentValidationProblemDetails>>() ).And.Result;
        result.Errors.Should().HaveCount(1);
        result.Errors["SerialNumber"][0].ErrorMessage.Should().Be("'Serial Number' must not be empty.");
    }

    [Fact]
    public async Task Should_Patch_A_Rocket_Type()
    {
        var client = new RocketClient(Factory.CreateClient());

        var rocket = await ServiceProvider.WithScoped<RocketDbContext>()
                                          .Invoke(
                                               async z =>
                                               {
                                                   var rocket = new ReadyRocket
                                                   {
                                                       Type = RocketType.AtlasV,
                                                       SerialNumber = "12345678901234"
                                                   };
                                                   z.Add(rocket);

                                                   await z.SaveChangesAsync();
                                                   return rocket;
                                               }
                                           );

        var u = await client.PatchRocketAsync(
            rocket.Id.Value,
            new EditRocketPatchRequest
            {
                Type = new() { Value = ClientRocketType.FalconHeavy }
            }
        );
        ( u.StatusCode is >= 200 and < 300 ).Should().BeTrue();

        var response = await client.GetRocketAsync(rocket.Id.Value);
        response.Result.Type.Should().Be(ClientRocketType.FalconHeavy);
        response.Result.Sn.Should().Be("12345678901234");
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
