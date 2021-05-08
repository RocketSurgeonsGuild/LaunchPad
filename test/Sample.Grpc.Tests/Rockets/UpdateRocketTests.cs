using Bogus;
using FluentAssertions;
using Grpc.Core;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Grpc.Tests.Validation;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using R = Sample.Grpc.Rockets;

namespace Sample.Grpc.Tests.Rockets
{
    public class UpdateRocketTests : HandleGrpcHostBase
    {
        private static readonly Faker Faker = new Faker();
        public UpdateRocketTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public async Task Should_Update_A_Rocket()
        {
            var client = new R.RocketsClient(Factory.CreateGrpcChannel());

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
            u.Should().NotBeNull();

            // var response = await client.GetRocketAsync(rocket.Id);
            //
            // response.Type.Should().Be(RocketType.FalconHeavy);
            // response.Sn.Should().Be("43210987654321");
        }

        [Theory]
        [ClassData(typeof(ShouldValidateUsersRequiredFieldData))]
        public void Should_Validate_Required_Fields(UpdateRocketRequest request, string propertyName)
        {
            var client = new R.RocketsClient(Factory.CreateGrpcChannel());
            Func<Task> a = async () => await client.EditRocketAsync(request);
            var e = a.Should().Throw<RpcException>().And;
                e.Status.StatusCode.Should().Be(StatusCode.InvalidArgument);
                e.Message.Should().Contain(propertyName);
        }

        class ShouldValidateUsersRequiredFieldData : TheoryData<UpdateRocketRequest, string>
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
}