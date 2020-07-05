using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.DependencyInjection;
using Sample.Core.Domain;
using Sample.Core.Operations.Rockets;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Core.Tests.Rockets
{
    public class UpdateRocketTests : HandleTestHostBase
    {
        private static readonly Faker Faker = new Faker();

        public UpdateRocketTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Trace) { }

        [Fact]
        public async Task Should_Update_A_Rocket()
        {
            var rocket = await _serviceProvider.WithScoped<RocketDbContext>()
               .Invoke(
                    async z =>
                    {
                        var rocket = new ReadyRocket()
                        {
                            Type = RocketType.Falcon9,
                            SerialNumber = "12345678901234"
                        };
                        z.Add(rocket);

                        await z.SaveChangesAsync();
                        return rocket;
                    }
                );

            var response = await _serviceProvider.WithScoped<IMediator>().Invoke(
                mediator => mediator.Send(
                    new EditRocket.Request(rocket.Id)
                    {
                        Type = RocketType.FalconHeavy,
                        SerialNumber = string.Join("", rocket.SerialNumber.Reverse())
                    }
                )
            );

            response.Type.Should().Be(RocketType.FalconHeavy);
            response.Sn.Should().Be("43210987654321");
        }
    }
}