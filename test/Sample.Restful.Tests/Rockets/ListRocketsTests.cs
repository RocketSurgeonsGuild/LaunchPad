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
    public class ListRocketsTests : HandleWebHostBase
    {
        private static readonly Faker Faker = new Faker();

        public ListRocketsTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Trace) { }

        [Fact]
        public async Task Should_List_Rockets()
        {
            var client = new RocketClient(Factory.CreateClient());
            await ServiceProvider.WithScoped<RocketDbContext>()
               .Invoke(
                    async z =>
                    {
                        var faker = new RocketFaker();
                        z.AddRange(faker.Generate(10));

                        await z.SaveChangesAsync();
                    }
                );

            var response = await client.ListRocketsAsync();

            response.Result.Should().HaveCount(10);
        }
    }
}