using Bogus;
using FluentAssertions;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Restful.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using RocketType = Sample.Core.Domain.RocketType;

namespace Sample.Restful.Tests.Rockets
{
    public class UpdateRocketTests : HandleWebHostBase
    {
        private static readonly Faker Faker = new Faker();
        public UpdateRocketTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

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

            var u = await client.UpdateRocketAsync(
                rocket.Id,
                new EditRocketModel
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

        [Theory]
        [ClassData(typeof(ShouldValidateUsersRequiredFieldData))]
        public void Should_Validate_Required_Fields(EditRocketModel request, string propertyName)
        {
            var client = new RocketClient(Factory.CreateClient());
            Func<Task> a = () => client.UpdateRocketAsync(Guid.NewGuid(), request);
            a.Should().Throw<ApiException<FluentValidationProblemDetails>>().And
               .Result.Errors.Values
               .SelectMany(x => x)
               .Select(z => z.PropertyName)
               .Should()
               .Contain(propertyName);
        }

        class ShouldValidateUsersRequiredFieldData : TheoryData<EditRocketModel, string>
        {
            public ShouldValidateUsersRequiredFieldData()
            {
                Add(
                    new EditRocketModel(),
                    nameof(EditRocketModel.SerialNumber)
                );
                Add(
                    new EditRocketModel { SerialNumber = Faker.Random.String2(0, 9) },
                    nameof(EditRocketModel.SerialNumber)
                );
                Add(
                    new EditRocketModel { SerialNumber = Faker.Random.String2(600, 800) },
                    nameof(EditRocketModel.SerialNumber)
                );
            }
        }
    }
}