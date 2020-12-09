using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Operations.Rockets;
using Sample.Restful.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using RocketType = Sample.Restful.Client.RocketType;

namespace Sample.Restful.Tests.Rockets
{
    public class CreateRocketTests : HandleWebHostBase
    {
        public CreateRocketTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public async Task Should_Create_A_Rocket()
        {
            var client = new RocketClient(Factory.CreateClient());

            var response = await client.CreateRocketAsync(
                new CreateRocketRequest()
                {
                    Type = RocketType.Falcon9,
                    SerialNumber = "12345678901234"
                }
            );

            response.Result.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Should_Throw_If_Rocket_Exists()
        {
            var client = new RocketClient(Factory.CreateClient());
            await client.CreateRocketAsync(
                new CreateRocketRequest()
                {
                    Type = RocketType.Falcon9,
                    SerialNumber = "12345678901234"
                }
            );

            Func<Task> action = () => client.CreateRocketAsync(
                new CreateRocketRequest()
                {
                    Type = RocketType.Falcon9,
                    SerialNumber = "12345678901234"
                }
            );
            var r = action.Should().Throw<ApiException<ProblemDetails>>()
               .And.Result;
            r.Title.Should().Be("Rocket Creation Failed");
        }
    }
    public class UpdateRocketTests : HandleWebHostBase
    {
        public UpdateRocketTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public async Task Should_Update_A_Rocket()
        {
            var client = new RocketClient(Factory.CreateClient());

            var rocket = await ServiceProvider.WithScoped<RocketDbContext>()
               .Invoke(
                    async z =>
                    {
                        var rocket = new ReadyRocket()
                        {
                            Type = Sample.Core.Domain.RocketType.Falcon9,
                            SerialNumber = "12345678901234"
                        };
                        z.Add(rocket);

                        await z.SaveChangesAsync();
                        return rocket;
                    }
                );

            var u = await client.UpdateRocketAsync(
                rocket.Id,
                new EditRocketRequest()
                {
                    Type = RocketType.FalconHeavy,
                    SerialNumber = string.Join("", rocket.SerialNumber.Reverse())
                }
            );
            ( u.StatusCode is >= 200 and < 300 ).Should().BeTrue();

            // var response = await client.GetRocketAsync(rocket.Id);
            //
            // response.Type.Should().Be(RocketType.FalconHeavy);
            // response.Sn.Should().Be("43210987654321");
        }
    }
}