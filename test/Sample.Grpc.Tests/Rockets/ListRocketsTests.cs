using Bogus;
using FluentAssertions;
using Rocket.Surgery.DependencyInjection;
using Sample.Core;
using Sample.Core.Domain;
using Sample.Grpc.Tests.Validation;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using R = Sample.Grpc.Rockets;

namespace Sample.Grpc.Tests.Rockets
{
    public class ListRocketsTests : HandleGrpcHostBase
    {
        private static readonly Faker Faker = new Faker();

        public ListRocketsTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        [Fact]
        public async Task Should_List_Rockets()
        {
            var client = new R.RocketsClient(Factory.CreateGrpcChannel());
            await ServiceProvider.WithScoped<RocketDbContext>()
               .Invoke(
                    async z =>
                    {
                        var faker = new RocketFaker();
                        z.AddRange(faker.Generate(10));

                        await z.SaveChangesAsync();
                    }
                );

            var response = await client.ListRocketsAsync(new ListRocketsRequest());

            response.Results.Should().HaveCount(10);
        }
    }
}