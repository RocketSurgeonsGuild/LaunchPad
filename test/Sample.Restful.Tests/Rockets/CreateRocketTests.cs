using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Sample.Core.Operations.Rockets;
using Sample.Restful.Client;
using System;
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
}