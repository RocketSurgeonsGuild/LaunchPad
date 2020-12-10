using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.DependencyInjection;
using Sample.Core;
using Sample.Core.Domain;
using Sample.Core.Operations.Rockets;
using Sample.Restful.Client;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Restful.Tests.Rockets
{
    public class RemoveRocketsTests : HandleWebHostBase
    {
        private static readonly Faker Faker = new Faker();

        public RemoveRocketsTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Trace) { }

        [Fact]
        public async Task Should_Remove_Rocket()
        {
            var client = new RocketClient(Factory.CreateClient());
            var id = await ServiceProvider.WithScoped<RocketDbContext>()
               .Invoke(
                    async z =>
                    {
                        var faker = new RocketFaker();
                        var rocket = faker.Generate();
                        z.Add(rocket);

                        await z.SaveChangesAsync().ConfigureAwait(false);
                        return rocket.Id;
                    }
                );

            await client.RemoveRocketAsync(id);

            ServiceProvider.WithScoped<RocketDbContext>().Invoke(c => c.Rockets.Should().BeEmpty());
        }
    }
}