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
    public class RemoveRocketsTests : HandleTestHostBase
    {
        private static readonly Faker Faker = new Faker();

        public RemoveRocketsTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Trace) { }

        [Fact]
        public async Task Should_Remove_Rocket()
        {
            var id = await _serviceProvider.WithScoped<RocketDbContext>()
               .Invoke(
                    async z =>
                    {
                        var faker = new RocketFaker();
                        var rocket = faker.Generate();
                        z.Add(rocket);

                        await z.SaveChangesAsync();
                        return rocket.Id;
                    }
                );

            await _serviceProvider.WithScoped<IMediator>().Invoke(
                mediator => mediator.Send(new DeleteRocket.Request() { Id = id})
            );

            _serviceProvider.WithScoped<RocketDbContext>().Invoke(c => c.Rockets.Should().BeEmpty());
        }
    }
}