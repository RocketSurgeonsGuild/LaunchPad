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
    public class GetRocketTests : HandleTestHostBase
    {
        private static readonly Faker Faker = new Faker();

        public GetRocketTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Trace) { }

        [Fact]
        public async Task Should_Get_A_Rocket()
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
                        return rocket.Id;
                    }
                );

            var response = await _serviceProvider.WithScoped<IMediator>().Invoke(
                mediator => mediator.Send(new GetRocket.Request() { Id = rocket })
            );

            response.Type.Should().Be(RocketType.Falcon9);
            response.Sn.Should().Be("12345678901234");
        }
    }
}